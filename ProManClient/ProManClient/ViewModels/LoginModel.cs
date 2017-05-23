using System.ComponentModel.DataAnnotations;

namespace ProManClient.ViewModels
{
    public class LoginModel
    {
        [Required( ErrorMessageResourceType = typeof( Resources.Strings.Site ), ErrorMessageResourceName = "lblUserNameRequired", ErrorMessage = null )]
        [Display( ResourceType = typeof( Resources.Strings.Site ), Name = "lblUserName" )]
        public string UserName { get; set; }

        [Required( ErrorMessageResourceType = typeof( Resources.Strings.Site ), ErrorMessageResourceName = "lblPasswordRequired", ErrorMessage = null )]
        [Display( ResourceType = typeof( Resources.Strings.Site ), Name = "lblPassword" )]
        [DataType( DataType.Password )]
        public string Password { get; set; }

        [Display( Name = "Remember me?" )]
        public bool RememberMe { get; set; }
    }
}