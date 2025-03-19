<!--     1         2         3         4         5         6         7       -->
<!--5678901234567890123456789012345678901234567890123456789012345678901234567-->
<h1>Introduction</h1>

L'application de bureau Nemeio est couramment appelée débarqué en interne, 
par opposition à l'embarqué qui constitue le code de pilotage du clavier 
(*firmware*).

L'objectif de cette application est d'assurer la liaison entre le clavier Nemeio 
et le système d'exploitation sur lequel il est branché.
Deux types de systèmes sont considérés initialement: les systèmes Windows 10 et
macOS. 
Par la suite le système *Linux* sera à considérer.

Cette application de bureau embarque à ce jour - pour ce qui concerne la version 
*Windows* - le navigateur *Chromium* afin d'afficher la partie *Configurateur*
qui constitue une application tierce mais déployée de façon intégrée.

# Table des matières
1. [Organisation](#organisation)<br>
1.1. [Philosophie](#philosophie)<br>
1.2. [Outils utilisés](#outils)<br>
1.2.1. [Outils Windows](#outils_windows)<br>
1.2.1.1. [Git](#outils_windows_git)<br>
1.2.1.2. [Code et installeurs](#outils_windows_code)<br>
1.2.1.3. [Icônes et images](#outils_windows_icone)<br>
1.2.1.4. [Documentation](#outils_windows_doc)<br>
1.2.2. [Outils macOS](#outils_macOS)<br>
1.3. [Le répertoire projet](#repertoire)<br>
1.3.1. [Récupération du répertoire](#repertoire_retrait)<br>
1.3.1. [Sous-modules](#repertoire_sous_modules)<br>
1.3.1. [Dépendances lourdes](#repertoire_lfs)<br>
1.4. [Les modules](#modules)<br>
1.4.1. [Les modules principaux](#modules_principaux)<br>
1.4.2. [Les modules Windows](#modules_windows)<br>
1.4.3. [Les modules macOS](#modules_macOS)<br>
1.4.4. [Les modules de test](#modules_tests)<br>
1.5. [Les installeurs](#installeurs)<br>
1.5.1. [Installation Windows](#installeurs_windows)<br>
1.5.1.1 [Installeur Windows](#installeurs_windows_regular)<br>
1.5.1.2 [Auto-installeur Windows](#installeurs_windows_auto)<br>
1.5.2. [Installation macOS](#installeurs_macOS)<br>
1.5.2.1. [Paquetage macOS](#installeurs_macOS_regular")<br>
1.5.2.2. [Auto-installeur macOS](#installeurs_macOS_auto")<br>
1.6. [Les pipelines](#pipelines)<br>
1.7. [La documentation](#documentation)<br>


<h1>1. Organisation<a name="organisation"></h1>

<h2>1.1. Philosophie<a name="philosophie"></h2>

Un unique répertoire versionné contient l'ensemble des informations nécessaires 
à la construction et la distribution du projet.<br>
Ceci comprends 
1. le code source qui est réalisés en C# et porté en Xamarin sous macOS
2. le code nécessaire pour la création des installeurs
3. les ressources nécessaire à la création des icônes
4. les pipelines Yaml et scripts associés
5. la documentation

<h2>1.2. Outils utilisés<a name="outils"></h2>

Sont listés dans cette section les divers outils nécessaires et/ou recommandés
pour gérer ce projet sous chaque plateforme.

<h3>1.2.1. Outils Windows<a name="outils_windows"></h3>

<h4>1.2.1.1 - Récupération du code Git<a name="outils_windows_git"></h4>

- <h5>Git</h5>

L'outil principal et initial est celui de ***Git*** pour Windows.

> https://gitforwindows.org/

Il permet de gérer le code aujourd'hui hébergé sous *Bitbucket*.
Ses consoles <*git bash*> sont très utiles pour un pilotage aisé en ligne 
de commande.

- <h5>Git-LFS</h5>

L'outil dérivé Git-LFS est également nécessaire pour télécharger et stocker les
ressources tierces binaires qui sont lourdes, qui évoluent peu et qui ne sont
pas gérées sous forme de dépendance *Nuget*...<br>
Ce module est particulièrement nécessaire pour générer les installeurs.

> https://git-lfs.github.com/

Il s'agit des dépendances des installeurs Windows liées au déploiement de 
*Chromium* qui n'est hélas pas géré comme une application tierce.<br>
Ce produit est donc actuellement intégré au Large File Storage qui contient
principalement le sous-dossier "*Nemeio.WindowsInstaller/Dependencies/*").<br>
Ce dernier représente ***342&nbsp;Mo*** de ressources embarquées...

**Attention**:

Les ressources "habituelles" de l'application telles que les icônes, images
et autre petits fichiers binaires sont stockés directement dans 
la solution.

- <h5>Fork (optionnel)</h5>

L'outil ***Fork*** est très utile pour une interface graphique intuitive:

> https://git-fork.com/
  
Il remplace aisément *SourceTree* (*Atlassian*) qui se montre souvent 
très capricieux, et reste disponible sous les deux plateformes 
*Windows* et *macOS*.

- <h5>TortoiseGit (optionnel)</h5>
  
L'outil ***TortoiseGit*** peut aussi être installé pour quelques
fonctionnalités utiles (historiques, diffs, *etc.*):

> <https://tortoisegit.org/>

<h4>1.2.1.2 - Compilation du code et installeurs<a name="outils_windows_code"></h4>

- <h5>Visual Studio Community Edition</h5>

L'outil principal à l'heure où ces lignes sont écrites est ***Visual Studio 
Community Edition 2019*** couramment référencé en tant que VS2019

> https://visualstudio.microsoft.com/fr/vs/

Voici les options d'installation requises:
<!-- Ignore Spelling: and, development, desktop, Platform, with, using, Core -->
* ASP.NET and web development
* .NET desktop development
* Universal Windows Platform development
* Mobile development with .NET (using Xamarin)
* Visual Studio Extension development
* .NET Core cross-platform development
<!-- Ignore Spelling: enregion pgragma /matchCase -->

- <h5>Wix Toolset</h5>

<!-- Ignore Spelling: Wix Toolset Build Tools -->
Afin de réaliser les installeurs Windows, l'outil ***Wix Toolset Build Tools***
doit être déployé.
<!-- Ignore Spelling: enregion pgragma /matchCase -->

> https://wixtoolset.org/releases/

- <h5>WixToolset Visual Studio Extension</h5>

Ce plugin n'est pas indispensable pour compiler l'installeur mais peut s'avérer
utile pour le modifier de façon intégrée à VS2019

> https://marketplace.visualstudio.com/items?itemName=WixToolset.WixToolsetVisualStudio2019Extension

Néanmoins, il sera probablement nécessaire de rentrer dans l'édition "manuelle" 
des fichiers <*.wxs*>

- <h5>Correcteur ortographique</h5>

Il peut s'avérer utile d'installer le module de correction orthographique suivant.
Il est notamment nécessaire pour vérifier l'orthographe de cette documentation
(en français dans le cas présent).

> https://marketplace.visualstudio.com/items?itemName=EWoodruff.VisualStudioSpellCheckerVS2017andLater

<h4>1.2.1.3 - Edition des icones et images SVG<a name="outils_windows_icone"></h4>

- <h5>Editeur SVG</h5>

Les référence d'images fournies par LDLC au format SVG sont éditables avec 
***Inkscape***

> https://inkscape.org/fr/

<h4>1.2.1.4 - Documentation<a name="outils_windows_doc"></h4>

- <h5>Markdown Editor</h5>

Il s’agit du plugin de Markdown qui s'intègre à VisualStudio afin notamment 
d'éditer cette documentation de façon intégrée.

> https://marketplace.visualstudio.com/items?itemName=MadsKristensen.MarkdownEditor

Mais d'autres outils externes de *Markdown* peuvent tout aussi bien être 
utilisés.

**Remarque:**

Il serait bon à terme d'intégrer cette documentation dans Confluence 
de façon automatique et aussi d'empêcher sa modification depuis
confluence, pour prévenir toute divergence ou perte d'information...

Une tache sur le serveur Azure pourrait idéalement générer la
documentation au format PDF ou HTML et également la publier au
bon endroit (il est possible dans ce cas que Confluence ne soit
plus la meilleure cible au vu de ses limitation actuelles.


- <h5>PlantUML</h5>

Pour l’édition des diagramme est schémas, l'outil ***PlantUML*** a été choisi 
pour sa simplicité.

> https://sourceforge.net/projects/plantuml/

Les diagrammes sont écrit au format XML avec l’extension "*.puml*" et peuvent 
donc être intégrés de façon textuelle à GIT.

<h3>1.2.2. Outils macOS<a name="outils_macOS"></h3>

< ... ***FixMe***: To be completed ... >


<h2>1.3. Le répertoire Projet<a name="repertoire"></h2>

Le conteneur de l'ensemble du projet sur bitbucket est accessible à l'adresse
suivante:

> <https://bitbucket.org/adeneo-embedded/b2047-ldlc-karmeliet-desktop-app/src/master/>

<h3>1.3.1 Obtention du code<a name="repertoire_retrait"></h5>

<!-- Ignore Spelling: recursive, adeneo-embedded, ldlc, karmeliet, checkout, lfs, update, install, submodule, init -->
Afin de récupérer ce conteneur il faut réaliser:

<sh>$ git clone --recursive git@bitbucket.org:adeneo-embedded/b2047-ldlc-karmeliet-desktop-app.git</sh>

L'option  **--recursive** ici assure la récupération des sous-modules et 
notamment le *configurateur* qui est géré séparément.

Le clone par défaut se positionne sur la branche "*master*" du dossier
et il est possible de basculer sur la branche développement avec la commande

<sh>$ git checkout development</sh>

<h3>1.3.2. Récupération du(des) sous-module(s)<a name="repertoire_sous_modules"></h3>

Si le clone récursif n'a pas été réalisé précédemment, il est possible de 
récupérer les sous-modules avec la commande

<sh>$ git submodule update --init</sh>

<h3>1.3.3. Récupération des dépendances lourdes<a name="repertoire_lfs"></h3>

Dans le cas où les installeurs Windows sont à générer, il faudra récupérer les
ressources stockées séparément sur git LFS.
Lorsque le module git-lfs a été déployé la toute première fois sur la 
machine, il faut lancer la commande:

<sh>$ git lfs install</sh>

Lorsque l'installation est réalisée, il faut récupérer les ressources de façon 
explicite avec

<sh>$ git lfs checkout</sh>

***Remarque:***

*La commande ci dessus est assez surprenante dans la mesure ou un git*
*update eut paru plus appropriée...*
<!-- Ignore Spelling: endregion pragma /matchCase -->

<h2>1.4. Les modules<a name="modules"></h2>

Le contenu du dossier git ainsi versionné contient entre autres les divers
modules de code.

<h3>1.4.1. Modules principaux<a name="modules_principaux"></h3>

Il s'agit des projets partagés par les deux plateformes et qui regroupent donc
le code "factorisé". 
Ces modules sont basés sur le *framework* ***.NET Standard 2.0***.

Leur découpage suit en principe une logique de dépendance sur des librairies
externes spécialisées selon l'une ou l'autre fonctionnalité.

![Nemeio modules partagés](./PlantUML/01-Organization-01-CommonModules.png)

<h4>Nemeio.Core</h4>

<!-- Ignore Spelling: Enums, Object-Relationnal Mapping, Entity, End, Access, Control -->
Ce module ne possède pas d'autres dépendances que celles du framework *.NET 
Standard 2.0* (et cela doit rester ainsi). Tous les autres modules dépendent
initialement de ce module.

Il s'agit de toutes les classes de base utilisées par toutes les plateformes
et de tous les mécanismes communs à ces plateformes (*Enums*, *Interface*, 
classes de base, éventuellement abstraites réalisant des mécanismes
communs...).<br>
Dans la mesure du possible un maximum de mécanismes doivent être ajoutés dans 
cet *Assembly* afin de garantir l'iso-fonctionnalité des plateformes.

<h4>Nemeio.Infrastructure</h4>

Ce module est principalement en charge de la base de données. Il se base sur EFCore et possède 
une dépendance sur *Microsoft.EntityFrameworkCore.Sqlite*.

Pour le fonctionnement de EF Core, suivre les indications de la documentation officielle Microsoft :
https://docs.microsoft.com/en-us/ef/core/


<h4>Nemeio.LayoutGen</h4>

Il s'agit ici du module permettant de générer dynamiquement les images liées à un
*layout*. Il est principalement lié et dépendant de SkiaSharp.

La génération d'un layout est réalisée à partir de l'interface ***IDeviceMap*** qui
est réalisée et donc spécialisée par chaque système.

<h4>Nemeio.API</h4>

Ce module prends en charge le serveur local destiné à interfacer
l'application de bureau avec le *Configurateur*. Il propose divers *End-Point*
documentés/documentables à l'aide de *Swagger*.

Ce serveur peut être sécurisé en https sous Windows bien que le mécanisme
soit actuellement suspendu. Il peut sembler quelque peu "superflu"
de vouloir sécuriser ce service en HTTPS alors qu'il n'est qu'à usage interne:
aucun échange n'est effectué par ce biais avec l'extérieur...

<h4>Nemeio.Acl</h4>

L'acronyme ACL représente *Access Control Layer*. Ce module est
originellement destiné à définir une couche d'accès au débarqué.
De fait, en plus de sa dépendance attendue sur Nemeio.Core, il possède
également une dépendance sur Nemeio.API, en raison de sa section HttpComm
qui semble néanmoins un peu limitée ici...
Il pourrait s'avérer judicieux de déporter cette section liée à HttpComm vers le
module API ou voire un autre module tiers et dédié.

La contenu principal du module ACL concerne la communication série entre le
logiciel et le clavier (et pourrait donc être rebaptisé e.g. *Nemeio.Serial*).
Ceci représente une dépendance principale sur divers modules de communication
série, commun à toutes les plateformes (*SerialPortStream*, *SerailPortLib*
et *System.IO.Ports*).

Un des principaux éléments de module est la classe ***SerialPortComm*** qui est
la classe pivot interfaçant le logiciel avec le clavier
(*WriteAsync* et *Read*).

<h4>Nemeio.Presentation</h4>

Le module présentation se veut le module de présentation commun à toutes les
plateformes.

Outre le fait qu'il embarque une copie du *Configurateur*, récupéré sous forme de
sous-module et distribué avec le débarqué, il définit des dépendances vers
* le framework MvvmCross dont il spécifie la classe ***Application*** (cette dépendance 
se voudrait être réduite à terme, éventuellement à travers MvvmLight?)
* le framework SeriLog afin de gérer le mécanisme de logging de l'application
(*cf.* la  classe ***Logger***)
<!-- Ignore Spelling: endregion pragma /matchCase -->

<h3>1.4.2. Les modules Windows<a name="modules_windows"></h3>

![Nemeio Windows modules](./PlantUML/01-Organization-02-WindowsModules.png)

La version Windows du débarqué se base sur les modules communs précédemment
cités, qui sont presque tous référencés par le nouveau module *Nemeio.Wpf*
qui est spécifique à Windows. Seul le module Nemeio.Api n'est pas
directement référencé.

Comme son nom l'indique, il s'agit de l'adaptation du logiciel à la
plateforme Windows avec notamment une implémentation en WPF (*Windows 
Presentation Framework*) pour ce qui concerne l’interface graphique.<br>
Cette adaptation WPF utilise notamment la classe ***NotifyIcon*** de la
dépendance tierce *Hardcodet.NotifyIcon.Wpf* car WPF en natif ne fournit
pas d'équivalent de la classe *TaskBarIcon* existant sous *WindForms*
(modèle originel).

Outre l’adaptation du logiciel à l'interface graphique Windows, ce module adapte
également tous les mécanismes tels que
<!-- Ignore Spelling: lock, unlock -->
* la manipulation des ports USB (*WMI* pour *Windows Management Instrumentation*)
* la manipulation des connexions BLE (*UWP* pour *Universal WindowsPlatfrom*)
* le suivit du statut de connexion Bluetooth
* la gestion des layouts propres au système Windows
* le suivi des applications actives
* le suivi des sessions utilisateur (*lock*/*unlock*)
* *etc.*
<!-- Ignore Spelling: endregion pragma /matchCase -->

Le projet *Nemeio.Wpf* est basé sur le *.NET Framework 4.6.1*

<h3>1.4.3. Modules macOS<a name="modules_macOS"></h3>

![Nemeio macOS modules](./PlantUML/01-Organization-03-macOSModules.png)

Similairement, la version *macOS* développée sous *Visual Studio Code* possède
les mêmes dépendances directes plus une dépendance sur un projet *XCode* qui
se trouve dans le dossier *WKeyboard*.

*WKeyboard* définit des fonctionnalités spécifique en *XCode* natif puis 
réimportées via le mécanisme *Dllimport* 

![Nemeio WKeyboard XCode module](./PlantUML/01-Organization-03-macOSModules-01-WKeyboard.png)

Le projet* Nemeio.Mac* est définit en tant que projet *Xamarin macOS*.

<h3>1.4.4. Modules de test<a name="modules_tests"></h3>

Pour chacun des modules C# listé ci-dessus (Partagé ou Windows ou macOS),
un projet de test unitaires dédiés est réalisé.

![Nemeio test modules](./PlantUML/01-Organization-04-TestModules.png)

Certains projets de test possède également une dépendance sur Nemeio.Core.Test
dans la mesure ou ce dernier définit des objets *Fake* qui implémentent certains
comportements nécessaire aux tests.<br> 
Cette pratique est à réduire dans la mesure du possible pour préférer l'usage 
d'objets *mockés*.

***Bonnes pratiques de nommage des tests***

***FixMe:*** https://adeneo-embedded.atlassian.net/browse/BLDLCK-2367

Le nom des tests, basés sur NUnit devraient idéalement suivre la syntaxe
suivante (en ligne bien-sûr):

1. <classe étant testée>_
2. [<ordonnanceur_de_méthode_optionnel>_]
3. <méthode étant testée>_
4. [<ordonnanceur_de_condition_optionnel>_]
5. <condition_de_test>_
6. <résultat_attendu_du_test>

Les "*ordonnanceurs*" optionnels visent juste à assurer qu'une classe est bien
testée dans le bon ordre depuis sa construction jusque vers ses méthodes les
plus complexes: les tests n'en sont que plus lisibles et il devient rassurant
de constater que un échec sur un constructeur, peut résulter dans l'échec
complet des méthodes suivantes.

Voici quelques exemples de nom de tests:

* *SerialPortComm_01_Constructor_01_InvalidArguments_Throws*
* *SerialPortComm_01_Constructor_02_ValidArguments_BuildOk*
* *SerialPortComm_02_StartAsync_01_SerialPortWontOpen_ReturnFalse*
* *etc.*

<h2>1.5. Les installeurs<a name="installeurs"></h2>

<h3>1.5.1. Installation Windows<a name="installeurs_windows"></h3>

Deux installeurs Windows sont à considérer:
* l'installeur de l’application elle même, Nemeio.WindowsInstalleur dont le
projet est accompagné d'un sous projet embarquant les "CustomActions"
* l'Auto-Installeur qui est embarqué sur le clavier et permet un téléchargement
ou une mise à jour automatique lors du branchement USB.

<h4>1.5.1.1 Installeur windows<a name="installeurs_windows_regular"></h4>

L'installeur Windows est réalisé avec l'outil tiers WIX pour une description XML
simplifiée du processus d'installation.

- <h5>Custom Actions</h5>

Originellement les CustomActions installaient et désinstallaient la tache
planifiée permettant le démarrage automatique de débarqué, mais ce mécanisme
a été abandonné pour le mécanisme plus simple de placer un raccourci Nemeio
dans le menu de démarrage de Windows (*StartUp Apps*).

Il reste le mécanisme toujours utilisé de nettoyage des dossiers
* "*Application Data*", qui contient essentiellement la base de donnée
* et "*Program Files*", qui embarque notamment une installation *implicite*
de chromium particulièrement problématique à la désinstallation.

En principe, les mécanismes natifs de *WIX* (et même de *Windows Installeur*)
devraient rendre possible de tels nettoyages sans usage de Custom Actions...

- <h5>Installeur WIX</h5>

<!-- Ignore Spelling: pre, text wixproj, Assemblies -->

Le projet principal d'installation dans *Nemeio.WindowsInstalleur* est décrit
par le fichier "*Product.Wxs*" ainsi que le *CommonComponentGroup* décrit dans
"*Common.Wxs*".

Deux installeurs distincts sont réalisés en fonction des plateformes cibles
<*x86*> et <*x64*>, toutes deux réalisées en *Release*.

D'autres dépendances sont générées lors du *pre-build* à l'aide de l'outil
<*Heat*>:
* *CommonDependencies.wxs*
* *Configurator.wxs*
* *x64.wxs* ou *x86.wxs* selon la plateforme.

Lorsqu'une dépendance sur l'un de ces sous-modules est modifiée, il faut
régénérer l'ensemble du <*wxs*> associé et pour cela, effacer le fichier dans
le dossier *Nemeio.WindowsInstalleur*; 
de fait lors du pre-build, l'absence du fichier étant détectée, il est
automatiquement régénéré par <*Heat*> avec de nouveaux <Guids> par
dépendance.

- <h5>Numéro de version</h5>

Afin de manipuler aisément les installeurs, le numéro de version est ajouté 
explicitement au nom de l'installeur.
Ceci est réalisé par l'ajout de la section suivante dans le <*wixproj*>:

```text
  <Target Name="BeforeBuild">
    <GetAssemblyIdentity AssemblyFiles="..\Nemeio.Wpf\bin\$(Platform)\$(Configuration)\Nemeio.exe">
      <Output TaskParameter="Assemblies" ItemName="AssemblyInfo" />
    </GetAssemblyIdentity>
    <CreateProperty Value="$(SolutionName)-$(Platform)-%(AssemblyInfo.Version)">
      <Output TaskParameter="Value" PropertyName="TargetName" />
    </CreateProperty>
  </Target>
```

- <h5>Signature</h5>

La signature digitale du programme et de ses installeurs ne s'effectue que
sur le serveur Azure. Cela permet notamment de préserver une plus grand
confidentialité du certificat de signature et de son mot de passe associé.

Cela nécessite cependant de pouvoir découper le projet de façon assez fine,
à savoir générer l'exécutable d'une part afin de le signer avec ses modules
associés (*Assemblies*).
Puis une fois tous ces fichier signés, générer l'installeur à proprement
parler en reprenant bien sûr les modules signés (et non pas en les régénérant).
<!-- Ignore Spelling: endregion pragma /matchCase -->

<h4>1.5.1.1 Auto-installeur windows<a name="installeurs_windows_auto"></h4>

L'auto-installeur Windows est destinés à télécharger la version la plus 
récente de l'application qui est associée au clavier courant. Cet 
auto-installeur se veut le plus minimaliste possible, dans le sens où il
est embarqué sur la mémoire persistante du clavier: il représente
actuellement environ <*50kb*> pour Windows.

Le numéro de série du clavier est récupéré sur le port USB puis,
le mini-installeur interroge le serveur de mises-à-jour avec ce numéro
de série afin de récupérer la version associée au clavier: 
cela permet d'associer les versions de test ou de développement
à des claviers dédiés.

A terme cela pourrait également permettre de distribuer des mises à jour
de façon séquencée selon une stratégie configurable
*e.g.* sur la première semaine, ne mettre à jour que les claviers dont
le numéro de série se termine par 1 et 2, puis rajouter de nouveaux
numéros de série dans les semaines suivantes...
Cela peut permettre entre autre de réduire la charge sur le serveur de
mises-à-jour (et donc de ne pas le surdimensionner inutilement).

Du reste l'auto-installeur affiche également une barre de progression 
du téléchargement en cours pour une meilleure expérience utilisateur.

<h3>1.5.2. Installation macOS<a name="installeurs_macOS"></h3>

Similairement à Windows, la plateforme macOS requiert deux projets 
distincts pour sa distribution:

<h4>1.5.2.1. Paquetage macOS<a name="installeurs_macOS_regular"></h4>

<!-- Ignore Spelling: dmg -->
Le paquetage macOS de type <.*dmg*> permet une "installation" simplifiée de 
l'applicatif sans passer par l'*Apple Store* qui constituerait le mécanisme 
usuel de distribution.
Le paquetage permet une installation simplifiée à travers un mécanisme de
glissé-déplacé.

La génération se fait à travers les commandes suivantes, qui sont opérée
depuis une console positionnée dans le dossier *Nemeio.MacInstalleur*:

<sh>$ ./build.sh</sh>

suivit de la commande

<sh>$ ./dmg-maker.sh</sh>

<!-- Ignore Spelling: endregion pragma /matchCase -->

<h4>1.5.2.2. Auto-installeur macOS<a name="installeurs_macOS_auto"></h4>

L'auto-installeur suit le même principe de minimisation de sa taille en raison
de son encombrement sur la mémoire limitée du clavier.
Il représente actuellement un contenu d'environ <*80kb*>.

Il s'agit du contenu du dossier Nemeio.MacAutoInstaller réalisé en *XCode* 
natif.

<h2>1.6. Les pipelines<a name="pipelines"></h2>

L'organisation du dossier Pipeline vise à versionner dans la mesure du possible les divers pipelines
réalisés dans le cadre de l'intégration CI/CD sur le serveur Azure du projet:

> https://dev.azure.com/devops0872/B2047%20-%20LDLC%20-%20Karmeliet

<!-- Ignore Spelling: common, installers, iso, builder, Inquiry, Cloud, completed, win, utils, versionning, publishing -->

Le serveur possède divers pipelines  avec extension <.*yaml*> qui sont classés
en 4 à 5 catégories:

* les pipelines communs (préfixés par *[Common]* ou "*common-*")
  * ***[Common] Unit Tests***, le pipeline de tests unitaires qui lancé pour
  chaque publication ou création de *PullRequest*, ou sur le *merge* d'une PR.
  * ***[Common] Installers***, le pipeline qui réalise les installeurs 
  (Windows x64, puis macOS, puis Windows x86) :   
  * ***[Common] Auto-Installers Iso Builder***, le pipeline qui réalise l'iso 
  déployé sur le clavier et qui embarque les auto-installeurs Windows et macOS
  * ***[Common] Update Inquiry***, < ... ***FixMe***: To be completed ... >
  * ***[Common] Sonar Cloud***, < ... ***FixMe***: To be completed ... >
* les pipelines spécifiques Windows, préfixés par (*[Windows]* ou *win-*)
  * ***[Windows] Application***, le pipeline qui génère l'installeur *<msi>*
  Windows
* les pipelines spécifiques macOS, préfixés par (*[Mac]* ou *mac-*)
  * ***[Mac] Application***, le pipeline qui génère le paquetage *<dmg>*
  pour macOS
* les pipelines utilitaires, préfixés par (*[Utils]* ou *utils-*)
  * ***[Utils] Release Notification***, permettant de notifier sur Slack la
  disponibilité d'une nouvelle version.
* les pipelines de développement (préfixés par *[Old]*)
  * les pipelines de cette catégorie ne sont pas persistés dans la solution 
  mais uniquement présents sur le serveur afin de ne pas perturber les 
  mécanismes usuels d'intégration continue.

Tous ces pipelines sont définis à la racine du dossier *<Pipeline>* et utilisent 
au besoin des templates définis dans le sous-dossier *<Templates>*.

Certains ***Jobs*** utilisent des scripts. ***Bash*** a été privilégié dans la
mesure du possible mais ils peuvent également contenir des scripts Python 
(qui requièrent alors la présence de Python sur le serveur d'intégration).<br>
Des sous-dossiers respectifs ont été ajoutés pour ces scripts, identifiant les
fonctionnalités réalisées *e.g.* <*Versionning*>, <*Publishing*>, *etc.*

<!-- Ignore Spelling: endregion pragma -->

<h2>1.7. La documentation<a name="documentation"></h2>

Le dossier *<Documentation>* contient la documentation embarquée et 
versionnée.<br> 
Le format simple retenu à ce jour est le format *Markdown* avec l'extension
*<.md>*.

Les diagrammes sont réalisés au format PlantUML, d'extension *<.puml>* se trouvent dans le sous-dossier
<*PlantUML*>. Les images au format <*.png*> sont ignorées et doivent donc être générées explicitement
via l'applicatif PlantUML.jar).


