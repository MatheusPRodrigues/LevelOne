using System.Text.Json;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Dialogflow.V2;

namespace LevelOne.Services;

public class DialogFlowService
{
    private readonly SessionsClient _sessionsClient;
    private readonly string _projectId;

    public DialogFlowService(IConfiguration configuration)
    {
        var directoryPath =
            Path.Combine(Directory.GetCurrentDirectory(), configuration["DialogFlow:CredentialPath"]);
        
        var jsonFile = Directory.GetFiles(directoryPath).FirstOrDefault();

        var credentialPath = Path.Combine(directoryPath, jsonFile);

        var credential = GoogleCredential.FromFile(credentialPath);
        var builder = new SessionsClientBuilder
        {
            Credential = credential
        };

        _sessionsClient = builder.Build();
        
        using var stream = new StreamReader(credentialPath);
        var json = JsonDocument.Parse(stream.ReadToEnd());
        _projectId = json.RootElement.GetProperty("project_id").GetString();
    }
    
    public async Task<string> SendMessageAsync(string sessionId, string message)
    {
        var session = new SessionName(_projectId, sessionId);

        var queryInput = new QueryInput
        {
            Text = new TextInput
            {
                Text = message,
                LanguageCode = "pt-BR"
            }
        };

        var response = await _sessionsClient.DetectIntentAsync(session, queryInput);
        return response.QueryResult.FulfillmentText;
    }
}