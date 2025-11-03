echo "🔄 Cloning GitHub main branch..."
git clone https://github.com/username/repoName.git
cd repoName
git checkout main

echo "📦 Pushing to Azure DevOps repo..."
git remote add azure https://$(System.AccessToken)@dev.azure.com/ProjektDotNET/Projekt%20.Net/_git/MortgageComparer
git push azure main --force
