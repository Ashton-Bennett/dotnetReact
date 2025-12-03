using Api.Models.DTOs;

namespace Api.Enums
{
    public static class EnumExtensions
    {
        public static List<RoleDto> ToRoleDtoList()
        {
            return Enum.GetValues(typeof(UserRole))
                .Cast<UserRole>()
                .Select(r => new RoleDto
                {
                    Id = (int)r,
                    Name = r.ToString()
                })
                .ToList();
        }
    }
}
