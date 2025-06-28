# Azure App Service Easy Auth Sample - Blazor Server

This is a Blazor Server application that demonstrates how to retrieve and display authentication information from Azure App Service Easy Auth (Authentication/Authorization).

## Features

- Displays authentication status
- Shows user information from Easy Auth headers
- Retrieves and displays tokens (ID token, Access token, Refresh token)
- Fetches detailed authentication information from the `/.auth/me` endpoint
- Shows all user claims

## How Easy Auth Works

Azure App Service Easy Auth provides authentication at the platform level. When enabled:

1. Users are redirected to an identity provider (Azure AD, Google, Facebook, etc.)
2. After successful authentication, App Service adds special headers to requests
3. The application can read these headers to get user information and tokens

## Key Headers Provided by Easy Auth

- `X-MS-CLIENT-PRINCIPAL-ID`: Unique identifier for the authenticated user
- `X-MS-CLIENT-PRINCIPAL-NAME`: Display name or email of the user
- `X-MS-CLIENT-PRINCIPAL-IDP`: Identity provider used (aad, google, facebook, etc.)
- `X-MS-TOKEN-AAD-ID-TOKEN`: Azure AD ID token (if using Azure AD)
- `X-MS-TOKEN-AAD-ACCESS-TOKEN`: Azure AD access token (if using Azure AD)
- `X-MS-TOKEN-AAD-REFRESH-TOKEN`: Azure AD refresh token (if using Azure AD)

## Deployment to Azure App Service

### Prerequisites

- Azure subscription
- Azure CLI or Azure Portal access
- .NET 8.0 SDK

### Deployment Steps

1. **Build the application**
   ```bash
   dotnet publish -c Release
   ```

2. **Create an Azure App Service** (if not already created)
   ```bash
   az webapp create --name your-app-name --resource-group your-rg --plan your-plan
   ```

3. **Deploy the application**
   ```bash
   az webapp deploy --name your-app-name --resource-group your-rg --src-path ./bin/Release/net8.0/publish/
   ```

   Or use Visual Studio/VS Code publish features.

4. **Enable Authentication/Authorization**
   - Go to Azure Portal
   - Navigate to your App Service
   - Click on "Authentication" in the left menu
   - Click "Add identity provider"
   - Choose your provider (e.g., Microsoft, Google, Facebook)
   - Configure the provider settings
   - Save the configuration

5. **Configure Token Store** (Important!)
   - In the Authentication settings
   - Click on your identity provider
   - Enable "Token store"
   - This allows the app to access tokens through headers

## Local Development

When running locally, the Easy Auth headers won't be present. The application will show "Not Authenticated" status. Easy Auth only works when deployed to Azure App Service.

## Testing

1. Deploy the application to Azure App Service
2. Configure authentication as described above
3. Access the application URL
4. You'll be redirected to authenticate
5. After successful authentication, you'll see your auth information displayed

## Troubleshooting

- **No tokens visible**: Ensure "Token store" is enabled in App Service authentication settings
- **/.auth/me returns 401**: Make sure you're accessing through the authenticated session
- **Headers missing**: Verify that authentication is properly configured in App Service

## Security Considerations

- Never expose tokens in production logs
- Be careful when displaying tokens in UI (this sample is for demonstration)
- Consider who has access to view these tokens
- Use HTTPS always

## Code Structure

- `Services/EasyAuthService.cs`: Service for retrieving Easy Auth information
- `Components/Pages/Home.razor`: Main page displaying auth information
- `Program.cs`: Service registration and app configuration