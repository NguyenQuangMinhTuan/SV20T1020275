using Microsoft.AspNetCore.Authentication.Cookies;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Security.Claims;
namespace SV20T1020275
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.ComponentModel.DataAnnotations;
    using System.Reflection;
    using Microsoft.AspNetCore.Authentication.Cookies;

    /// <summary>
    /// Thông tin tài khoản người dùng được lưu trong phiên đăng nhập
    /// </summary>
    public class WebUserData
    {
        public string? UserId { get; set; }
        public string? UserName { get; set; }
        public string? DisplayName { get; set; }
        public string? Email { get; set; }
        public string? Photo { get; set; }
        public string? ClientIP { get; set; }
        public string? SessionId { get; set; }
        public string? AdditionalData { get; set; }
        public List<string>? Roles { get; set; }

        /// <summary>
        /// Thông tin người dùng dưới dạng danh sách các Claim
        /// </summary>
        /// <returns></returns>
        private List<Claim> Claims
        {
            get
            {
                List<Claim> claims = new List<Claim>()
            {
                new Claim(nameof(UserId), UserId ?? ""),
                new Claim(nameof(UserName), UserName ?? ""),
                new Claim(nameof(DisplayName), DisplayName ?? ""),
                new Claim(nameof(Email), Email ?? ""),
                new Claim(nameof(Photo), Photo ?? ""),
                new Claim(nameof(ClientIP), ClientIP ?? ""),
                new Claim(nameof(SessionId), SessionId ?? ""),
                new Claim(nameof(AdditionalData), AdditionalData ?? "")
            };

                if (Roles != null)
                {
                    foreach (var role in Roles)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, role));
                    }
                }

                return claims;
            }
        }

        /// <summary>
        /// Tạo Principal dựa trên thông tin của người dùng
        /// </summary>
        /// <returns></returns>
        public ClaimsPrincipal CreatePrincipal()
        {
            var claimIdentity = new ClaimsIdentity(Claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimPrincipal = new ClaimsPrincipal(claimIdentity);
            return claimPrincipal;
        }
    }

    /// <summary>
    /// Thông tin về nhóm/quyền
    /// </summary>
    public class WebUserRole
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="name">Tên/ký hiệu nhóm/quyền</param>
        /// <param name="description">Mô tả</param>
        public WebUserRole(string name, string description)
        {
            Name = name;
            Description = description;
        }

        /// <summary>
        /// Tên/Ký hiệu quyền
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Mô tả
        /// </summary>
        public string Description { get; set; }
    }

    /// <summary>
    /// Danh sách các nhóm quyền sử dụng trong ứng dụng
    /// </summary>


    /// <summary>
    /// Extension các phương thức cho các đối tượng liên quan đến xác thực tài khoản người dùng
    /// </summary>
    public static class WebUserExtensions
    {
        /// <summary>
        /// Lấy thông tin tài khoản người dùng từ ClaimsPrincipal
        /// </summary>
        /// <param name="principal"></param>
        /// <returns></returns>
        public static WebUserData? GetUserData(this ClaimsPrincipal principal)
        {
            try
            {
                if (principal == null || principal.Identity == null || !principal.Identity.IsAuthenticated)
                    return null;

                var userData = new WebUserData();
                userData.UserId = principal.FindFirstValue(nameof(userData.UserId));
                userData.UserName = principal.FindFirstValue(nameof(userData.UserName));
                userData.DisplayName = principal.FindFirstValue(nameof(userData.DisplayName));
                userData.Email = principal.FindFirstValue(nameof(userData.Email));
                userData.Photo = principal.FindFirstValue(nameof(userData.Photo));
                userData.ClientIP = principal.FindFirstValue(nameof(userData.ClientIP));
                userData.SessionId = principal.FindFirstValue(nameof(userData.SessionId));
                userData.AdditionalData = principal.FindFirstValue(nameof(userData.AdditionalData));
                userData.Roles = new List<string>();

                foreach (var claim in principal.FindAll(ClaimTypes.Role))
                {
                    userData.Roles.Add(claim.Value);
                }

                return userData;
            }
            catch
            {
                return null;
            }
        }
    }

}
