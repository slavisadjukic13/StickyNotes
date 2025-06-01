using StickyNotes.Dtos;
using StickyNotes.Models;

namespace StickyNotes.Mapping
{
    public interface IUserMapper
    {
        UserResponseDto MapUserToUserResponse(User user);
        User MapUserResponseToUser(UserResponseDto userResponse);

        UserUpdateDto MapUserToUpdateUserRequest(User user);

        User MapUpdateUserRequestToUser(UserUpdateDto updateUserRequest);


    }

    public class UserMapper : IUserMapper
    {
        public UserResponseDto MapUserToUserResponse(User user)
        {
            return new UserResponseDto
            {
                Id = user.Id,
                Username = user.Username,
                // TODO: IsAdmin = user.IsAdmin,
                IsDisabled = user.IsDisabled
            };

        }

        public User MapUserResponseToUser(UserResponseDto userResponse)
        {
            return new User
            {
                Id = userResponse.Id,
                Username = userResponse.Username,
                // TODO: IsAdmin = userResponse.IsAdmin,
                IsDisabled = userResponse.IsDisabled
            };
        }

        public UserUpdateDto MapUserToUpdateUserRequest(User user)
        {
            return new UserUpdateDto
            {
                Id = user.Id,
                // TODO: IsAdmin = !user.IsAdmin,
                IsDisabled = user.IsDisabled
            };
        }

        public User MapUpdateUserRequestToUser(UserUpdateDto updateUserRequest) {
            return new User
            {
                Id = updateUserRequest.Id,
                // TODO: IsAdmin = !updateUserRequest.IsAdmin,
                IsDisabled = updateUserRequest.IsDisabled

            };
        }
    }
}
