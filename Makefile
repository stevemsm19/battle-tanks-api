API_PROJECT = src/BattleTanks.Api
INFRA_PROJECT = src/BattleTanks.Infrastructure

CYAN := \033[36m
YELLOW := \033[33m
NC := \033[0m

help:
	@awk 'BEGIN {FS = ":.*?## "} /^[a-zA-Z_-]+:.*? ## / {printf "$(CYAN)%-20s$(NC) %s\n", $$1, $$2}' $(MAKEFILE_LIST)

run: ## Run the application with hot reload
	@echo "$(YELLOW)Starting application...$(NC)"
	dotnet run --project $(API_PROJECT)

migration: ## Create a new migration
	@echo "$(YELLOW)Creating migration $(name)...$(NC)"
	dotnet ef migrations add $(name) --project $(INFRA_PROJECT) --startup-project $(API_PROJECT)

db-update: ## Update the database
	@echo "$(YELLOW)Updating database...$(NC)"
	dotnet ef database update --project $(INFRA_PROJECT) --startup-project $(API_PROJECT)

reset-all: ## Clean and rebuild the application
	@echo "$(YELLOW)Cleaning and rebuilding application...$(NC)"
	dotnet clean
	dotnet build
	dotnet ef database update --project $(INFRA_PROJECT) --startup-project $(API_PROJECT)
