using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Supervaga.Models;

namespace Supervaga.Controllers
{
    [ApiController]
    [Route("v1/ads")]
    public class AdController : ControllerBase
    {
        [HttpGet]
        [Route("page/{page:int}/limit/{limit:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Ad>>> Get(
            [FromServices] DataContext context,
            [FromRoute] int page = 0,
            [FromRoute] int limit = 25
        )
        {
            try
            {
                var ads = new List<Ad>();

                var count = await context.Ads.CountAsync();

                foreach (var ad in context.Ads)
                    ads = await context.Ads
                        .Include(x => x.User)
                        .Include(x => x.Advantages.Where(a => a.AdId == ad.Id))
                        .Include(x => x.Requirements.Where(r => r.AdId == ad.Id))
                        .Skip(page * limit)
                        .Take(limit)
                        .AsNoTracking()
                        .ToListAsync();


                if (ads == null)
                    return NoContent();

                return Ok(new { ok = true, count, data = ads });
            }
            catch (Exception e)
            {
                return BadRequest(
                    new
                    {
                        ok = false,
                        msg = e.Message
                    }
                );
            }
        }

        [HttpGet]
        [Route("{id:Guid}")]
        [AllowAnonymous]
        public async Task<ActionResult<Ad>> Get(
            [FromServices] DataContext context,
            Guid id
        )
        {
            try
            {
                var ad = await context.Ads
                .Include(x => x.User)
                .Include(a => a.Requirements.Where(r => r.AdId == id))
                .Include(a => a.Advantages.Where(a => a.AdId == id))
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id);

                if (ad == null)
                    return NoContent();

                return Ok(new { ok = true, data = ad });
            }
            catch (Exception e)
            {
                return BadRequest(
                    new
                    {
                        message = $"Ocorreu um erro ao obter a vaga referente a {id}: { e.Message}"
                    }
                );
            }
        }

        [HttpPost]
        [Route("")]
        [Authorize]
        public async Task<ActionResult<Ad>> Post(
            [FromServices] DataContext context,
            [FromBody] Ad model
        )
        {
            if (!ModelState.IsValid)
                return BadRequest(new { ok = false, data = ModelState });

            try
            {
                var user = await context.Users.FirstOrDefaultAsync(x => x.Id == model.UserId);

                context.Ads.Add(model);
                await context.SaveChangesAsync();

                model.User = user;

                return Ok(new { ok = true, data = model });
            }
            catch (Exception e)
            {
                return BadRequest(new { ok = false, message = e.ToString() });
            }
        }

        [HttpPut]
        [Route("{id:Guid}")]
        [Authorize]
        public async Task<ActionResult<Ad>> Put(
            [FromServices] DataContext context,
            [FromBody] Ad model,
            Guid id
        )
        {
            if (!ModelState.IsValid)
                return BadRequest(new { ok = false, data = ModelState });

            if (model.Id != id)
                return NotFound(new { ok = false, data = "Vaga não encontrada" });

            try
            {
                await _removeRequirementAndAdvantage(context, id);

                await _addRequirementAndAdvantage(context, model);

                context.Entry<Ad>(model).State = EntityState.Modified;
                await context.SaveChangesAsync();

                return Ok(new { ok = false, data = model });
            }
            catch (DbUpdateConcurrencyException dbEx)
            {
                return BadRequest(new { ok = false, message = dbEx.Message });
            }
            catch (Exception e)
            {
                return BadRequest(new { ok = false, message = e.ToString() });
            }
        }

        [HttpDelete]
        [Route("{id:Guid}")]
        [Authorize]
        public async Task<ActionResult<string>> Delete(
            [FromServices] DataContext context,
            Guid id
        )
        {
            try
            {
                var ad = await context.Ads.FirstOrDefaultAsync(x => x.Id == id);

                if (ad == null)
                    return NotFound(new { ok = false, message = "Vaga não encontrada" });

                await _removeRequirementAndAdvantage(context, id);

                context.Ads.Remove(ad);
                await context.SaveChangesAsync();

                return Ok(new { ok = true, message = $"Vaga com id {ad.Id} apagada" });
            }
            catch (Exception e)
            {
                return BadRequest(new { ok = false, message = e.Message });
            }
        }

        private async Task _addRequirementAndAdvantage(DataContext context, Ad model = null)
        {
            model.Requirements.ToList().ForEach(r => context.Requirements.Add(r));
            model.Advantages.ToList().ForEach(a => context.Advantages.Add(a));
            await context.SaveChangesAsync();
        }

        private async Task _removeRequirementAndAdvantage(DataContext context, Guid id)
        {
            var advs = context.Advantages.Where(x => x.AdId == id).ToList();
            var reqs = context.Requirements.Where(x => x.AdId == id).ToList();

            reqs.ForEach(x => context.Remove(x));
            await context.SaveChangesAsync();

            advs.ForEach(x => context.Remove(x));
            await context.SaveChangesAsync();
        }
    }
}