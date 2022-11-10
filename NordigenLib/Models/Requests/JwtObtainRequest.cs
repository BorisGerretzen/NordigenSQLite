namespace NordigenLib.Requests;

public class JwtObtainRequest {
    public required string SecretId { get; set; }
    public required string SecretKey { get; set; }

    public Dictionary<string, string> AsDictionary() => new() {
        { "secret_id", SecretId },
        { "secret_key", SecretKey }
    };
}