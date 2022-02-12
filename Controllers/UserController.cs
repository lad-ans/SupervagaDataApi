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
    [Route("v1/users")]
    public class UserController : ControllerBase
    {
        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<dynamic>> Authenticate(
            [FromServices] DataContext context,
            [FromBody] User model
        )
        {
            if (!ModelState.IsValid)
                return BadRequest(new { ok = false, message = ModelState });

            try
            {
                var user = await context.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Email == model.Email && x.Password == model.Password);

                var token = TokenService.GenerateToke(user);

                user.Password = "";

                return Ok(new { ok = true, data = new { user = user, token = token } });
            }
            catch (Exception e)
            {
                return BadRequest(new { ok = false, message = e.Message });
            }
        }

        [HttpGet]
        [Route("")]
        [Authorize]
        public async Task<ActionResult<List<Ad>>> Get(
            [FromServices] DataContext context,
            [FromQuery] int page = 0,
            [FromQuery] int limit = 25
        )
        {
            try
            {
                var count = await context.Users.CountAsync();

                var users = await context.Users
                    .AsNoTracking()
                    .Skip(page * limit)
                    .Take(limit)
                    .ToListAsync();

                if (users == null)
                    return NoContent();

                return Ok(new { ok = true, count, data = users });
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
        public async Task<ActionResult<Ad>> Get(
            [FromServices] DataContext context,
            Guid id
        )
        {
            try
            {
                var user = await context.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (user == null)
                    return NoContent();

                return Ok(new { ok = true, data = user });
            }
            catch (Exception e)
            {
                return BadRequest(
                    new
                    {
                        message = $"Ocorreu um erro ao obter o usuário referente a {id}: { e.Message}"
                    }
                );
            }
        }

        [HttpPost]
        [Route("")]
        public async Task<ActionResult<User>> Post(
            [FromServices] DataContext context,
            [FromBody] User model
        )
        {
            if (!ModelState.IsValid)
                return BadRequest(new { ok = false, data = ModelState });

            try
            {
                context.Users.Add(model);
                await context.SaveChangesAsync();

                return Ok(new { ok = true, data = model });
            }
            catch (Exception e)
            {
                return BadRequest(new { ok = false, message = e.ToString() });
            }
        }

        [HttpPut]
        [Route("{id:Guid}")]
        public async Task<ActionResult<User>> Put(
            [FromServices] DataContext context,
            [FromBody] User model,
            Guid id
        )
        {
            if (!ModelState.IsValid)
                return BadRequest(new { ok = false, data = ModelState });

            if (model.Id != id)
                return NotFound(new { ok = false, data = "Usuário não encontrada" });

            try
            {
                context.Entry<User>(model).State = EntityState.Modified;
                await context.SaveChangesAsync();

                return Ok(new { ok = false, data = model });
            }
            catch (DbUpdateConcurrencyException dbEx)
            {
                return BadRequest(new { ok = false, message = dbEx.Message });
            }
            catch (Exception e)
            {
                return BadRequest(new { ok = false, message = e.Message });
            }
        }

        [HttpDelete]
        [Route("{id:Guid}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<string>> Delete(
            [FromServices] DataContext context,
            Guid id
        )
        {
            try
            {
                var user = await context.Users.FirstOrDefaultAsync(x => x.Id == id);

                if (user == null)
                    return NotFound(new { ok = false, message = "Usuário não encontrada" });

                context.Users.Remove(user);
                await context.SaveChangesAsync();

                return Ok(new { ok = true, message = $"Usuário com id {user.Id} apagado" });
            }
            catch (Exception e)
            {
                return BadRequest(new { ok = false, message = e.Message });
            }
        }
    }
}