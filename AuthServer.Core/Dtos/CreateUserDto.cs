namespace AuthServer.Core.Dtos
{
    public class CreateUserDto
    {
        /// <summary>
        /// hidayatarg@gmail.com
        /// </summary>
        public string UserName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }
    }
}