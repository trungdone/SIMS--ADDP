namespace SIMS_App.Models
{
    public class User
    {
        public int Id { get; set; }          // Mã định danh duy nhất
        public int UserId { get; set; }
        public string Username { get; set; } // Tên đăng nhập
        public string Password { get; set; } // Mật khẩu (có thể mã hóa)
        public string Name { get; set; }     // Họ tên
        public string Email { get; set; }    // Email liên hệ
        public string Phone { get; set; }    // Số điện thoại
        public DateTime DateOfBirth { get; set; } // Ngày sinh
        public string Role { get; set; }     // Vai trò (Admin, User, etc.)
    }

}




