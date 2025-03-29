# Git Workflow

Try as far as possible to keep to these steps when adding to the project:

1. Create *specific* Issue describing the task you will be working on **or** Find relevant issue describing the task
- Under Issues -> New Issue
2. Create a branch for the issue **or** Link an existing branch
- Under *IssueName* -> Development -> Create a branch **or** *BranchName*
- Branch should have the same/similar name
- Try as far as possible to only make changes that pertain to the specified issue
3. When completeted, Create a Pull Request
- Under Pull Requests -> New Pull Request
- Select *BranchName* on the right hand side
- Should look something like base:master <- compare:branchname
4. Resolve conflicts
- If there are conflicts there will be red text reading "Can't automatically merge"
5. Once there are no conflicts, merge into master
- Ideally use the option "Squash and Merge"