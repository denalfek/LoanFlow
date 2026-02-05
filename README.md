# LoanFlow

A loan application processing system built with .NET 9.0, PostgreSQL, and RabbitMQ.

## Getting Started

### Development

To run the application locally using Docker Compose:

```bash
docker-compose up
```

The API will be available at `http://localhost:5100`.

### Production Deployment

For production deployment to Azure, GitHub secrets must be configured. See [GITHUB_SECRETS.md](GITHUB_SECRETS.md) for detailed instructions on setting up the required secrets.

#### Required GitHub Secrets

- Azure configuration (CLIENT_ID, TENANT_ID, SUBSCRIPTION_ID, WEBAPP_NAME)
- Database connection string
- RabbitMQ credentials

See the complete list in [GITHUB_SECRETS.md](GITHUB_SECRETS.md).
