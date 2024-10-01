public class PasswordRecoveryEmailDTO
{
    public string Email { get; set; }
}

public class PasswordRecoveryCodeDTO
{
    public string Code { get; set; }
}

public class PasswordChangeDTO
{
    public string NewPassword { get; set; }
    public string ConfirmNewPassword { get; set; }
}
