using System.ComponentModel.DataAnnotations;

public class UpdateUsernameDto
{
    [Required]
    public string NewUsername { get; set; }
}

public class UpdatePasswordDto
{
    [Required]
    [DataType(DataType.Password)]
    public string NewPassword { get; set; }
}
