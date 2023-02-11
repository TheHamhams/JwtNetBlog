using JwtNetBlog.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JwtNetBlog.Controllers
{
    [Route("api/post")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly DataContext _context;

        public PostController(DataContext context)
        {
            _context = context;
        }

        // GET requests
        [HttpGet]
        public async Task<ActionResult<List<Post>>> GetAll()
        {
            return Ok(await _context.Posts.ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<List<Post>>> SeePost(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            var posts = _context.Posts.Where(p => p.UserId == id).ToList();
            if (posts == null)
            {
                return BadRequest("No post by that User found");
            }

            return Ok(posts);
        }

        // POST requests
        [HttpPost]
        public async Task<ActionResult<Post>> CreatePost(Post request)
        {
            var user = await _context.Users.FindAsync(request.UserId);
            if (user == null)
            {
                return BadRequest("User not found");
            }

            var post = new Post 
            { 
                UserId = user.Id,
                Title= request.Title,
                Description = request.Description,
                Created = DateTime.UtcNow,
            };

            _context.Posts.Add(post);
            user.Posts.Add(post);

            await _context.SaveChangesAsync();

            return Ok(post);
        }

        // DELETE requests
        [HttpDelete("{id}")]
        public async Task<ActionResult<string>> DeletePost(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return BadRequest("Post ID not found");
            }

            var user = await _context.Users.FindAsync(post.UserId);
            if (user == null) 
            { 
                return NotFound();
            }

            _context.Posts.Remove(post);
            user.Posts.Remove(post);

            await _context.SaveChangesAsync();

            return Ok("Post deleted");
        }
    }
}
