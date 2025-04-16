# Contributing to GrowthBook OpenFeature Provider for .NET

Thank you for your interest in contributing to the GrowthBook OpenFeature Provider for .NET!

## Development Environment Setup

1. **Prerequisites**:
   - .NET 8 SDK
   - An IDE (Visual Studio, VS Code with C# extension, or JetBrains Rider)

2. **Clone the repository**:

   ```bash
   git clone https://github.com/growthbook/growthbook-openfeature-provider-dot-net.git
   cd growthbook-openfeature-provider-dot-net
   ```

3. **Restore dependencies**:

   ```bash
   dotnet restore
   ```

4. **Build the solution**:

   ```bash
   dotnet build
   ```

5. **Run tests**:

   ```bash
   dotnet test
   ```

## Making Changes

1. **Create a branch**:

   ```bash
   git checkout -b feature/your-feature-name
   ```

2. **Implement your changes**:
   - Follow the existing code style and patterns
   - Add/update tests as needed
   - Ensure all tests pass

3. **Run the sample app** (optional):

   ```bash
   cd GrowthBook.OpenFeature.SampleApp
   dotnet run
   ```

## Code Guidelines

- Follow standard C# naming conventions and code style
- Use meaningful variable and method names
- Add XML documentation comments to public APIs
- Write unit tests for new functionality
- Keep methods small and focused on a single responsibility

## Submitting a Pull Request

1. **Commit your changes**:

   ```bash
   git commit -m "Description of your changes"
   ```

2. **Push your branch**:

   ```bash
   git push origin feature/your-feature-name
   ```

3. **Create a pull request**:
   - Go to the repository on GitHub
   - Click "New pull request"
   - Select your branch
   - Fill out the PR template

4. **Code review**:
   - Maintainers will review your code
   - Address any feedback or requested changes
   - Once approved, your PR will be merged

## Release Process

The maintainers follow this process for releases:

1. Update version in project file
2. Update CHANGELOG.md
3. Create a GitHub release
4. Publish to NuGet

## Questions?

If you have questions about contributing, please open an issue on GitHub.
