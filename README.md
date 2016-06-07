# Bomberman-VR

##Informations pratiques
- Nous utilisons l'extension '*Git Large File Storage*' pour le stockage de gros fichiers sur GitHub.
- Les projets Unity laissent souvent trainer des fichiers inutiles même après une suppression rapide, pour avoir quelque chose de propre, nous utilisons un script Powershell de l'utilisateur GitHub '*Strich*' nommée '*post-merge*'.

##Configuration correcte du Repo GitHub
1. Cloner la branche Master
2. Télécharger le [Git-Dir-Cleaner-for-Unity3D] (https://github.com/strich/git-dir-cleaner-for-unity3d), puis placer le fichier '*post-merge*' dans ".git/hooks/"
3. Installer [Git Large File Storage] (https://git-lfs.github.com/)
4. Ouvrir un '*Shell Git*', se placer dans le dossier cloné, exécuter la commande `git lfs install`
5. Tout est bon !