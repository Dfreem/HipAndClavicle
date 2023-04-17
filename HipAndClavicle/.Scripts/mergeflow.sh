#!/bin/bash
read "this command should be executed from your topic branch that is ready to submit for review.

currentBranch=$(git branch --show-current) 
git checkout main 
git fetch origin
git pull origin main
  if [[ $? = 0 ]]; then 
git checkout $currentBranch 
echo "currently on $currentBranch"
fi

# git add -A; 
# git rm $(git ls-files --deleted) 2> /dev/null; 
# git commit --no-verify --no-gpg-sign --message "merging $currentBranch" 
# git push 
exit 0
