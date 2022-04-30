# FundMon
Une application simple de suivi des cours de portefeuilles

#Fonctions
FundMon est une application WinUI3 qui permet de suivre les fonds (OPCVM) regroupés dans des portefeuilles. 

Les portefeuilles peuvent être créés à volonté. Ils peuvent contenir autant de lignes de fonds que nécessaire. Chaque ligne de fond comporte une référence à un fond et un coût moyen d'acquisition. L'historique des fonds peut être récupéré par un compte Morningstar.

Le suivi permet de comparer pour chaque fond le coût moyen d'acquisition et les valeurs du jour, de la semaine précédente et du mois précédent pour fournir une base d'arbitrage.

L'historique de chaque fond peut être représenté par un graphique modulaire.

#Structure

##Test unitaires
Les tests unitaires utilisent xUnit. La configuration des tests a été modifiée manuellement pour être compatible avec les paramètres imposés par WinUI3 :
* TargetFramework fixé à net6.0-windows10.0.19041.0
* Cible de compilation fixée à x86 pour éviter les erreurs de génération
