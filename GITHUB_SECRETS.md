# GitHub Secrets Configuration for Production

This document outlines the GitHub secrets required for the LoanFlow application's production deployment.

## Required GitHub Secrets

To deploy the LoanFlow application to production, you need to configure the following secrets in your GitHub repository settings (`Settings` > `Secrets and variables` > `Actions`).

### Azure Configuration

These secrets are used for authenticating and deploying to Azure:

| Secret Name | Description | Example Value |
|------------|-------------|---------------|
| `AZURE_CLIENT_ID` | Azure Service Principal Client ID | `12345678-1234-1234-1234-123456789012` |
| `AZURE_TENANT_ID` | Azure Active Directory Tenant ID | `87654321-4321-4321-4321-210987654321` |
| `AZURE_SUBSCRIPTION_ID` | Azure Subscription ID | `abcdefab-abcd-abcd-abcd-abcdefabcdef` |
| `AZURE_WEBAPP_NAME` | Name of the Azure Web App | `loanflow-prod` |

### Production Database Configuration

These secrets configure the PostgreSQL database connection for production:

| Secret Name | Description | Example Value |
|------------|-------------|---------------|
| `PROD_DATABASE_CONNECTION_STRING` | PostgreSQL connection string | `Host=prod-db.postgres.database.azure.com;Database=loanflow_prod;Username=loanadmin;Password=<secure-password>;SslMode=Require` |
| `PROD_DATABASE_AUTO_MIGRATE` | Whether to automatically run migrations on startup | `false` (recommended for production) |

### Production RabbitMQ Configuration

These secrets configure the RabbitMQ message broker for production:

| Secret Name | Description | Example Value |
|------------|-------------|---------------|
| `PROD_RABBITMQ_HOST` | RabbitMQ server hostname | `rabbitmq.example.com` or Azure Service Bus endpoint |
| `PROD_RABBITMQ_PORT` | RabbitMQ server port | `5672` (default AMQP port) |
| `PROD_RABBITMQ_USERNAME` | RabbitMQ username | `loanflow_prod` |
| `PROD_RABBITMQ_PASSWORD` | RabbitMQ password | `<secure-password>` |
| `PROD_RABBITMQ_VIRTUALHOST` | RabbitMQ virtual host | `/` (default) or `/loanflow-prod` |

## Setting Up Secrets

### Via GitHub Web UI

1. Navigate to your repository on GitHub
2. Go to `Settings` > `Secrets and variables` > `Actions`
3. Click `New repository secret`
4. Enter the secret name and value
5. Click `Add secret`

### Via GitHub CLI

```bash
# Azure Configuration
gh secret set AZURE_CLIENT_ID --body "your-client-id"
gh secret set AZURE_TENANT_ID --body "your-tenant-id"
gh secret set AZURE_SUBSCRIPTION_ID --body "your-subscription-id"
gh secret set AZURE_WEBAPP_NAME --body "your-webapp-name"

# Database Configuration
gh secret set PROD_DATABASE_CONNECTION_STRING --body "your-connection-string"
gh secret set PROD_DATABASE_AUTO_MIGRATE --body "false"

# RabbitMQ Configuration
gh secret set PROD_RABBITMQ_HOST --body "your-rabbitmq-host"
gh secret set PROD_RABBITMQ_PORT --body "5672"
gh secret set PROD_RABBITMQ_USERNAME --body "your-rabbitmq-username"
gh secret set PROD_RABBITMQ_PASSWORD --body "your-rabbitmq-password"
gh secret set PROD_RABBITMQ_VIRTUALHOST --body "/"
```

## Security Best Practices

1. **Never commit secrets to the repository** - All sensitive values should be stored as GitHub secrets
2. **Use strong passwords** - Generate secure, random passwords for database and RabbitMQ
3. **Rotate secrets regularly** - Update secrets periodically and after any security incidents
4. **Principle of least privilege** - Grant only necessary permissions to service principals and database users
5. **Enable SSL/TLS** - Always use encrypted connections for database and RabbitMQ in production
6. **Disable auto-migration in production** - Set `PROD_DATABASE_AUTO_MIGRATE` to `false` and run migrations manually
7. **Monitor secret usage** - Review GitHub Actions logs to ensure secrets are being used correctly

## Environment-Specific Configuration

### Development
- Uses `appsettings.Development.json`
- Configured via `docker-compose.yml` with development credentials
- No secrets required

### Production (Azure Deployment)
- Uses `appsettings.Production.json` as base
- Overridden by environment variables set from GitHub secrets
- All sensitive data comes from GitHub secrets

### Production (Docker Compose)
- Uses `docker-compose.production.yml`
- Requires environment variables to be set (see `.env.example`)
- Can use `.env` file locally or environment variables in deployment
- PostgreSQL and RabbitMQ credentials are also configured via environment variables

## Troubleshooting

### Deployment fails with "Secret not found"
- Verify all required secrets are configured in GitHub repository settings
- Ensure secret names match exactly (they are case-sensitive)

### Application fails to connect to database
- Verify the `PROD_DATABASE_CONNECTION_STRING` is correct
- Ensure the database server allows connections from Azure Web App
- Check that SSL/TLS requirements are properly configured

### Application fails to connect to RabbitMQ
- Verify all RabbitMQ configuration secrets are correct
- Ensure the RabbitMQ server is accessible from Azure Web App
- Check firewall rules and network security groups

## Additional Resources

- [GitHub Actions - Using secrets in GitHub Actions](https://docs.github.com/en/actions/security-guides/using-secrets-in-github-actions)
- [Azure - Deploy from GitHub Actions](https://docs.microsoft.com/en-us/azure/app-service/deploy-github-actions)
- [PostgreSQL Connection Strings](https://www.npgsql.org/doc/connection-string-parameters.html)
- [RabbitMQ Configuration](https://www.rabbitmq.com/configure.html)
