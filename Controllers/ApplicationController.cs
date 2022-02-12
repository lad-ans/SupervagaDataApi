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
    [Route("v1/applications")]
    public class ApplicationClass : ControllerBase
    {
        [HttpGet]
        [Route("")]
        public async Task<ActionResult<List<Application>>> Get(
            [FromServices] DataContext context
        )
        {
            try
            {
                var apps = new List<Application>();

                foreach (var app in context.Applications)
                    apps = await context.Applications
                        .Include(x => x.Ad)
                        .Include(x => x.Ad.User)
                        .Include(x => x.Ad.Advantages.Where(y => y.AdId == app.AdId))
                        .Include(x => x.Ad.Requirements.Where(y => y.AdId == app.AdId))
                        .Include(x => x.Candidates.Where(y => y.ApplicationId == app.Id))
                        .AsNoTracking()
                        .ToListAsync();

                if (apps == null)
                    return NoContent();

                return Ok(new { ok = true, data = apps });
            }
            catch (Exception e)
            {
                return BadRequest(new { ok = false, message = e.Message });
            }
        }

        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<ActionResult<Application>> Get(
            [FromServices] DataContext context,
            Guid id
        )
        {
            try
            {
                var app = new Application();

                var _app = await context.Applications.FirstOrDefaultAsync(x => x.Id == id);

                if (_app == null)
                    return NotFound(new { ok = false, message = "Candidatura não encontrada" });

                app = await context.Applications
                    .Include(x => x.Ad)
                    .Include(x => x.Ad.User)
                    .Include(x => x.Ad.Advantages.Where(x => x.AdId == _app.AdId))
                    .Include(x => x.Ad.Requirements.Where(x => x.AdId == _app.AdId))
                    .Include(x => x.Candidates.Where(y => y.ApplicationId == _app.Id))
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (app == null)
                    return NoContent();

                return Ok(new { ok = true, data = app });
            }
            catch (Exception e)
            {
                return BadRequest(new { ok = false, message = e.Message });
            }
        }

        [HttpPost]
        [Route("")]
        [Authorize]
        public async Task<ActionResult<Application>> Post(
            [FromServices] DataContext context,
            [FromBody] Application model
        )
        {
            if (!ModelState.IsValid)
                return BadRequest(new { ok = false, data = ModelState });

            try
            {
                context.Applications.Add(model);
                await context.SaveChangesAsync();

                model.Ad = await context.Ads
                    .Include(x => x.User)
                    .Include(x => x.Advantages.Where(y => y.AdId == model.AdId))
                    .Include(x => x.Requirements.Where(y => y.AdId == model.AdId))
                    .FirstOrDefaultAsync(x => x.Id == model.AdId);

                return Ok(new { ok = true, data = model });
            }
            catch (Exception e)
            {
                return BadRequest(new { ok = false, message = e.Message });
            }
        }

        [HttpPut]
        [Route("{id:Guid}")]
        [Authorize]
        public async Task<ActionResult<Application>> Put(
            [FromServices] DataContext context,
            [FromBody] Application model,
            Guid id
        )
        {
            if (!ModelState.IsValid)
                return BadRequest(new { ok = false, data = ModelState });

            if (model.Id != id)
                return NotFound(new { ok = false, message = "Candidatura não encontrada" });

            try
            {
                var cands = context.Candidates.ToList();

                cands.ForEach(x => context.Candidates.Remove(x));
                await context.SaveChangesAsync();

                context.Candidates.AddRange(model.Candidates);
                await context.SaveChangesAsync();

                context.Entry<Application>(model).State = EntityState.Modified;
                await context.SaveChangesAsync();

                model.Candidates = context.Candidates.ToList();

                model.Ad = context.Ads
                    .Include(x => x.User)
                    .Include(x => x.Advantages.Where(y => y.AdId == model.AdId))
                    .Include(x => x.Requirements.Where(y => y.AdId == model.AdId))
                    .FirstOrDefault(x => x.Id == model.AdId);

                return Ok(new { ok = true, data = model });
            }
            catch (DbUpdateConcurrencyException DEx)
            {
                return BadRequest(new { ok = false, message = DEx.ToString() });
            }
            catch (Exception e)
            {
                return BadRequest(new { ok = false, message = e.Message });
            }
        }
    }

}