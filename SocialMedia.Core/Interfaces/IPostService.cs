using SocialMedia.Core.Entities;
using System.Threading.Tasks;

namespace SocialMedia.Core.Interfaces
{
    public interface IPostService
    {
        Task InsertPost(Post post);
    }
}