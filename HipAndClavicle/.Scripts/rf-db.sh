#!/bin/bash

cd "$(pwd)"

# Prompt the user for a name for the new migration
read -p "Enter a name for the new migration: " migration_name

echo "rebuilding migrations and database. See .log file for errors and messages"

# Drop the database 
dotnet ef database drop -f > .log

# Remove the last migration 

echo "removed old migrations and database"

dotnet ef migrations remove > .log

echo "building database"

# Add a new migration with the user input as the name 
dotnet ef migrations add $migration_name > .log

# Update the database with the new migration 
dotnet ef database update > .log

<<<<<<< HEAD
echo "success"
=======
echo "success"
>>>>>>> 1f86abf4896a501a5e9520aa6c388dbe8d9ec0f5
