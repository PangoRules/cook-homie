# Documentation Synchronization Approach

## Overview

Documentation synchronization ensures that project documentation remains accurate and up-to-date with the evolving codebase. This approach focuses on automatically updating API documentation, architecture documents, and developer guides (like READMEs) after development work is completed and reviewed.

## Core Principles

1. **Proactive Updates**: Documentation should be updated as part of the development workflow, not as an afterthought
2. **Trigger-Based Automation**: Updates occur in response to specific events like code reviews or task completion
3. **Selective Synchronization**: Only documentation affected by changes gets updated
4. **Verification First**: Documentation changes are verified before being considered complete

## Integration with Development Workflow

The documentation synchronization process fits naturally into the existing development cycle:

1. **Development Phase**: Implement features or fix bugs
2. **Review Phase**: Submit code for review using the requesting-code-review skill
3. **Synchronization Trigger**: After review approval, automatically check for documentation impacts
4. **Update Phase**: Modify affected documentation sections
5. **Verification**: Run documentation verification checks
6. **Completion**: Mark work as complete with updated documentation

## Documentation Types and Update Strategies

### API Documentation
- **Source**: C# XML comments, OpenAPI/Swagger generation
- **Update Mechanism**: 
  - Regenerate OpenAPI specs from code annotations
  - Update reference documents with new/changed endpoints
  - Verify example requests/responses match implementation
- **Tools**: Swagger/OpenAPI generators, documentation-writer skill

### Architecture Documents
- **Source**: Component diagrams, data flow descriptions, technology decisions
- **Update Mechanism**:
  - Review architectural decisions made during implementation
  - Update component relationship diagrams if interfaces changed
  - Revise technology stack descriptions if new dependencies added
  - Update data model descriptions if schema changed
- **Tools**: Documentation-writer skill, manual review for complex changes

### Developer Guides (README, CONTRIBUTING, etc.)
- **Source**: Setup instructions, contribution guidelines, development workflows
- **Update Mechanism**:
  - Update setup steps if dependencies or configuration changed
  - Modify contribution guidelines if new processes introduced
  - Revise development workflow documentation if tooling changed
  - Update troubleshooting sections with new known issues
- **Tools**: Documentation-writer skill, template-based updates

## Implementation Approach

### 1. Change Impact Analysis
After code review approval but before marking work complete:
- Analyze which files were modified
- Determine documentation sections likely affected by those changes
- Prioritize updates based on impact severity

### 2. Automated Detection (Where Possible)
- Use scripts to detect changes in:
  - API controller files → flag API docs for update
  - Architecture decision records → flag architecture docs
  - Dependency files (package.json, csproj) → flag setup docs
  - Configuration files → flag configuration documentation

### 3. Manual Review and Update
For complex changes requiring judgment:
- Documentation writer reviews change summary
- Identifies which document sections need modification
- Creates or updates content using established patterns
- Ensures technical accuracy and consistency

### 4. Verification Checklist
Before considering documentation work complete:
- [ ] API docs match actual endpoints and responses
- [ ] Architecture diagrams reflect current component relationships
- [ ] Setup instructions work in clean environment
- [ ] Contribution guidelines match actual processes
- [ ] No broken links or outdated references

## Best Practices

### For API Documentation
- Keep XML comments updated alongside implementation changes
- Use automated spec generation as single source of truth
- Document breaking changes prominently
- Include version information when applicable

### For Architecture Documents
- Update decision records when architectural choices change
- Keep diagrams simplified but accurate
- Cross-reference with code where appropriate
- Note deprecated components or patterns

### For Developer Guides
- Test setup instructions regularly
- Use inclusive language welcoming contributors of all levels
- Keep getting-started guides under 10 minutes to verify
- Include common troubleshooting scenarios

## Integration with Existing Skills

This approach works synergistically with:
- **requesting-code-review**: Triggers documentation check after approval
- **verification-before-completion**: Includes documentation verification in completion checks
- **finishing-a-development-branch**: Ensures docs are updated before branch integration
- **writing-plans**: Can include documentation tasks in implementation plans

## Example Workflow

1. Developer completes add inventory item feature
2. Requests code review using requesting-code-review skill
3. Reviewer approves changes
4. System triggers documentation synchronization check
5. Documentation writer examines changes:
   - API controller modified → updates API reference docs
   - New database migration → updates architecture data model section
   - Added environment variable → updates .env.example and setup guide
6. Documentation writer makes necessary updates
7. Runs verification checks (builds, link checks, etc.)
8. Marks work complete with confidence documentation is accurate

## Benefits

- **Reduced Documentation Debt**: Prevents accumulation of outdated docs
- **Increased Trust**: Team knows documentation reflects current state
- **Faster Onboarding**: New contributors get accurate information
- **Better Maintenance**: Easier to debug and extend well-documented systems
- **Professionalism**: Consistent, accurate documentation reflects quality

## Considerations

- **Balance Automation with Judgment**: Not all changes require doc updates; use discretion
- **Maintain Documentation Ownership**: Developers remain responsible for doc accuracy
- **Start Small**: Begin with critical paths (API, setup) before expanding scope
- **Regular Audits**: Schedule periodic reviews to catch drift

By integrating documentation synchronization into the review completion workflow, CookHomie ensures its documentation remains a reliable asset rather than becoming outdated technical debt.