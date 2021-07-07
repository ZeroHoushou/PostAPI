﻿using SocialMedia.Core.Entities;
using SocialMedia.Core.Exceptions;
using SocialMedia.Core.Interfaces;
using SocialMedia.Core.QueryFilters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMedia.Core.Services
{
    public class PostService : IPostService
    {
        private readonly IUnitOfWork _unitOfWork;
        public PostService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

       

        public IEnumerable<Post> GetPosts(PostQueryFilter filters)
        {
            var posts =  _unitOfWork.PostRepository.GetAll();
        
            if(filters.UserId !=null)
            {
                posts = posts.Where(x=>x.UserId == filters.UserId);
            }
            if(filters.Date != null)
            {
                posts = posts.Where(x => x.Date.ToShortDateString() ==filters.Date?.ToShortDateString());
            }
            if (filters.Description != null)
            {
                posts = posts.Where(x => x.Description.ToLower().Contains(filters.Description.ToLower()));
            }

                return posts;
        
        }

        public async Task<Post> GetPost(int id)
        {
            return await _unitOfWork.PostRepository.GetByid(id);
        }

        public async Task InsertPost(Post post)
        {
            var user = await _unitOfWork.UserRepository.GetByid(post.UserId);
            if (user == null)
            {
                throw new BussinesException("Usuario no existe"); //Excepcion de dominio personalizada
            }
            var userPost = await _unitOfWork.PostRepository.GetPostsByUser(post.UserId);
            if(userPost.Count() < 10)
            {
                var lastPost = userPost.OrderByDescending(x => x.Date).FirstOrDefault();
                if((DateTime.Now - lastPost.Date  ).TotalDays < 7)
                {
                    throw new BussinesException(" You are not able to publish ");
                }
            }
            if (post.Description.Contains("Sexo"))
            {
                throw new BussinesException("Content not allowed");
            }

            await _unitOfWork.PostRepository.Add(post);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> UpdatePost(Post post)
        {
            _unitOfWork.PostRepository.Update(post);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DeletePost(int id)
        {
            await _unitOfWork.PostRepository.Delete(id);
            return true;
        }
    }
}
