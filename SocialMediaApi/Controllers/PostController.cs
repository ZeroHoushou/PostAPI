using Microsoft.AspNetCore.Mvc;
using SocialMedia.Core.Entities;
using SocialMedia.Core.Interfaces;
using System.Threading.Tasks;
using SocialMedia.Core.DTOs;
using System.Linq;
using AutoMapper;
using System.Collections.Generic;

namespace SocialMediaApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {

        private readonly IPostRepository _postRepository;
        private readonly IMapper _mapper; 
        public PostController(IPostRepository postRepository, IMapper mapper)
        {
            _postRepository = postRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetPosts()
        {
            var posts = await _postRepository.GetPosts();
            var postsDto = _mapper.Map<IEnumerable<PostDto>>(posts);
            return Ok(postsDto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPost(int id)
        {
            var post = await _postRepository.GetPost(id);
            var postsDto = _mapper.Map<PostDto>(post);
            return Ok(postsDto);
        }

        [HttpPost]
        public async Task<IActionResult> Post(PostDto postDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }


            var post = _mapper.Map<Post>(postDto); 
            
            await _postRepository.InserPost(post);
            return Ok(post);
        }
    }
}
