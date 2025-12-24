@echo off
git init
git add .
git commit -m "Force push EmotionCore"
git branch -M main
git remote remove origin
git remote add origin https://github.com/Chaitanyahoon/EmotionCore
git push -u origin main
echo DONE
