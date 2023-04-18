#!/usr/bin/bash
read -p "this command should be executed from your topic branch that is ready to submit for review. Are you on 
your 
topic branch? y/n" onTopic
if [[ $onTopic = 'y' ]]; then
  currentBranch=$(git branch --show-current) 
  git checkout main 
  git fetch origin
  git pull origin main
else
  echo "please switch off of your main branch"
  exit 1
fi

if [[ $? = 0 ]]; then 
  git checkout $currentBranch
  echo "success! currently on branch $currentBranch"
fi

# git add -A; 
# git rm $(git ls-files --deleted) 2> /dev/null; 
# git commit --no-verify --no-gpg-sign --message "merging $currentBranch" 
# git push 
exit 0
