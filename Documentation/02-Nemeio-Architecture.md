<!--     1         2         3         4         5         6         7       -->
<!--5678901234567890123456789012345678901234567890123456789012345678901234567-->

<!-- Ignore Spelling: blabla, Bla -->
<!-- Ignore Spelling: enregion pragma /matchCase -->

<h1>Introduction</h1>

L'architecture du projet Nemeio est une architecture *a posteriori* introduite
dans un projet de type prototype, grâce à diverses évolutions puis
réorganisations du code.<br>
Bien que la plupart des grandes fonctionnalités à ce jour semblent
clairement identifiées et séparées, le travail n'est jamais
définitivement terminé et peut toujours contenir des réorganisations en
cours ou à venir.

Le document actuel d'architecture, vise dans un premier temps à présenter
les entités principales du projet débarqué et leur interactions, ainsi que la
philosophie préconisée.
L'ordre d'apparition des classes selon le démarrage du programme sera suivi
dans la mesure du possible.


<!--
# Table des matières
1. [Organisation](#organisation)<br>
-->

<h1>Core</h1>

<h2>Préambule</h2>

<!-- Ignore Spelling: Controllers, Inject -->

<h3>Injection explicite</h3>

L'usage de ***Mvx.Resolve*** dans le code afin de résoudre l'accès à des
implémentations d'interfaces, présente l'inconvénient de masquer au relecteur
et à l'utilisateur la dépendance de la classe appelante sur cette interface.
Ceci de façon plus évidente lorsque la résolution est réalisée en plein
milieu d'un long bloc de code.

Afin de pallier à cet inconvénient, il est préféré - dans la mesure du possible -
d'injecter explicitement les dépendances dans le constructeur des classes -
utilisatrices (fréquemment les *Controllers* et *Managers*, mais pas 
uniquement).

Bien sûr cette règle n'est pas absolue et il est parfois nécessaire de réaliser
une injection différée lorsque l'injection de construction n'est pas
possible. Ce mécanisme est alors explicitement identifié en préfixant le nom
de la méthode d'injection par *Inject* e.g.

<!-- Ignore Spelling: enregion pragma /matchCase -->

<sh>_displayController.InjectLayoutManager(...)</sh>

<h3>Factory système</h3>

<!-- Ignore Spelling: core, acl, factory, Devices -->

La volonté de supporter plusieurs plateformes (Windows et macOS à ce jour) ont
conduit à la duplication de nombreux mécanismes. Si le code n'est pas toujours
partageable entre les plateforme, leur divergence conduit bien souvent à des
divergences de fonctionnalité, ce qui n'est pas souhaitable.

De fait bon nombre de mécanismes sont le plus possible développés au sein de
modules communs (*Core*, *Infrastructure*, *Api*, *Acl*, *LayoutGen*) puis
spécialisés selon la plateforme finale lorsque cela s'avère nécessaire.
Cette spécialisation est souvent réalisée par un mécanisme d'héritage qui permet
de mettre dans la classe parente le code "*partageable*" puis de surcharger
selon les spécificités de chaque système.

De fait la création de certaines instances de classes qui sont initialement
effectuées dans les modules de bas niveau (Core principalement mais les autres
aussi), peut et doit recourir à l'interface *ISystemKeyboardFactory*.
cette classe se trouve actuellement dans *Core/Devices* mais une réorganisation
de ce module permettrait sans doute une plus grande clarté quant au "rangement"
des classes.

Voici un aperçu de l'interface en question qui pourra donner une idée de quelles
implémentation d'interface dépendent du système.

![ISystemKeyboardFactory](./PlantUML/02-Architecture-01-Core-01-ISystemKeyboardFactory.png)

<!-- Ignore Spelling: enregion -->

<h2>Le contrôleur d'application</h2>

Le démarrage de l'application est lancé par la création et le démarrage du
contrôleur d'application. Ce dernier est instancié parmi les dernières classes
afin de pouvoir profiter de l'injection de dépendance explicite des autres 
classes.

![ApplicationController](./PlantUML/02-Architecture-01-Core-02-ApplicationController.png)

<!-- Ignore Spelling: start, shutdown, debug -->

Le mécanisme de *Start()/Stop()* se retrouvera fréquemment sur des classes 
ayant vocation à être suspendues durant leur existence.
Le contrôleur d'application quant à lui se termine par un *Shutdown()* mettant 
fin à toute l'application.

**Remarque:**

Sous Windows, en raison du démarrage  de l'application sous forme d'icône dans
la barre des tâches, l'aperçu visuel de l'icône peut persister alors que
l'application est arrêtée: plus particulièrement, lorsque l'application
est tuée (en mode Debug ou à travers le gestionnaire de tâche),
ou sur une erreur système.

C'est lors de la fermeture "propre" de l'application que la méthode Shutdown
est explicitement appelée et qu'elle assure le retrait de l'icône de l'interface
graphique. L'absence d'appel à cette fermeture "propre" engendre le symptôme
de persistance de l'icône, qui n'est pas résolu à ce jour.

Le fait de passe la souris au dessus de l'icône provoque le rafraîchissement
de la barre des tâche et fait alors disparaître l'icône fantôme. Aucun autre
mécanisme ne permet aujourd'hui de pallier ce problème. Il faudrait envisager
un mécanisme tiers et indépendant, ce qui dépasse l'objectif de notre
applicatif ici.

<!-- Ignore Spelling: enregion -->

<h2>Le gestionnaire des langues</h2>

Afin de gérer l'internationalisation du clavier, un certain nombre de langues
cibles ont été spécifiées par LDLC.
Il a été choisi de mettre en place un mécanisme de gestionnaire de langues,
qui permet de traduire des identifiants de message *StringId* selon 
la langue couramment sélectionnée pour l'application.

Cette préférence de langue pourrait être paramétrée à travers le configurateur.
Elle est initialement définie à partir de la préférence de la langue régionale
définie sur la session de l'utilisateur.

Grâce au gestionnaire de langues, un changement de langue "à chaud"
 (sans redémarrage de l'application) est tout à fait envisageable.

![ApplicationController](./PlantUML/02-Architecture-01-Core-03-LanguageManager.png)

L'association de l'identifiant de message StringId avec le message lui-même
est mémorisé dans un fichier XML par langue.
Il sera aisé pour faciliter la traduction de fournir le fichier anglais de
référence et le fichier de la langue à traduire (soit dupliqué de l'anglais soit
vide) et de versionner ces différents fichiers de ressource.

**Remarques:**

- Dans un premier temps, l'application embarque toutes les traductions 
disponibles mais à terme, des fichiers de ressource "enfichables" voire
même téléchargeables à la demande pourrait être considérés.
- Les installeurs eux-même sont traduits et embarquent des langues 
d'installation dans des fichiers *a priori* distincts à ce jour.
Aucun mécanisme n'a été envisagé afin de partager les fichiers de traduction
de l'application et de son installeur. La remarque est aussi valable pour les
auto-installeurs (embarqués sur le clavier). 
Il sera néanmoins bon de considérer que la traduction de ces trois types
d'applicatif (débarqué, installeur et auto-installeur) devront
 bénéficier d'une seul et unique  opération de traduction.

<h2>Le gestionnaire d'affichage</h2>

<h3>La classe abstraite <i>DisplayController</i></h3>

Le gestionnaire d'affichage est destiné à gérer les retours utilisateur à
travers la GUI. Chaque implémentation doit donc se faire spécifiquement au
système visé (*Windows* ou *macOS*), mais bon nombre de mécanismes communs
sont factorisés au sein de la classe abstraite *DisplayController* qui implémente
l'interface toute simple de notification.

Lui sont injectés divers gestionnaires (parfois en post-injection) afin de 
tenir l'utilisateur informé du statut de ces derniers (connexion, mises à jour
clavier sélectionné et/ou sélectionnable, etc.)

![DisplayController](./PlantUML/02-Architecture-01-Core-04-DisplayController-01.png)

<h3>La spécialisation par système</h3>

<!-- FixMe: il manque la référence de la technologie utilisée pour la GUI macOS-->
Par la suite chaque système adapte ces mécanismes commun à travers leur 
implémentation et leur interface spécifique. C'est là notamment que les
technologies diffèrent: *WPF* pour *Windows* et <?> pour *macOS*.

![DisplayController](./PlantUML/02-Architecture-01-Core-04-DisplayController-02.png)

<h2>Le gestionnaire de layouts</h2>

<h3>Gestion des layouts</h3>

Le gestionnaire de Layout joue un rôle essentiel dans l'exécution du débarqué:
c'est lui qui assure la synchronisation entre la sélection de la langue système
et la sélection de la langue sur le clavier. C'était un des premiers rôles dévolu
au débarqué d'assurer cette synchronisation avec les claviers HID.
Lorsqu'un clavier utilisateur (Custom) est sélectionné, c'est alors le clavier
système qui lui est associé qui est synchronisé sur le système.

Trois mécanismes explicites de changement de layout sont impliqués ici:
- le défilement des layouts sur le clavier lui-même à l'aide des boutons arrière
"°" et "°°"
- le choix de langue à travers le menu système (sous *Windows*, il est également 
possible de changer de clavier avec la combinaison touches "*Windows*" +
"*Espace*")
- le choix de langue à travers le menu Nemeio lui-même

Un quatrième mécanisme - implicite - peut également changer le clavier affiché:
il s'agit du mécanisme d'association d'un layout à une ou plusieurs applications.
<br>
Lors que l'une des applications associée à un clavier devient active sur le
système, alors le clavier en question est directement activé par le débarqué.

<h3>Class abstraite <i>LayoutManager</i></h3>

Une première abstraction du gestionnaire de Layout implémente l'interface
associée de façon factorisée.

<!-- Ignore Spelling: create, read, update, delete -->
Cette instance contient entre autre une instance d'un ILayoutCrudManager qui est
la couche de persistance des layouts (CRUD est l'acronyme de 
*Create-Read-Update-Delete*).
Cette instance est totalement encapsulée et n’apparaît donc pas dans 
l'interface de la classe.
<!-- Ignore Spelling: enregion -->

Le gestionnaire de layout possède quelques propriétés publiques 
(attention à bien garder les setters privés!):
- *CurrentLayoutId*: qui contient l'identifiant du layout actuellement
sélectionné sur le clavier, et l'application. Dans le cas d'un layout
utilisateur, ce layout ne peut-être synchronisé sur le système: on lui
préférera de fait le layout associé.
- *DefaultLayoutId*: par définition, le configurateur doit identifier 
un layout comme étant le Layout par défaut. *N.B.* *cette propriété ne semble
plus vraiment utilisée à ce jour!?!*
- *ApplicationLayoutManager*: un accesseur au gestionnaire de layouts associés
aux application. C'est ce dernier qui gère l'activation d'un layout en fonction
de l'application courante renseignée par le *ProcessInformation*, dans la mesure
ou il ne s'agira pas d'une application black listée.
- *LayoutList*: une liste des layouts actuellement mémorisés dans le débarqué.
Cette liste est supposée synchronisée sur le clavier et c'est l'une des premières
opération réalisée lors de la connexion à une clavier: vérification de
la différence entre les layouts définis côté débarqué et les layouts présents
sur le clavier; suppression des layouts non attendus (éventuellement désactivés
bine que présents en base); ajouts des layouts manquants.
<!-- Ignore Spelling: started -->
- *Started*: un état démarré ou arrêté du gestionnaire. A vérifier
mais il est probable que ce statut soit en quelques sorte obsolète
aujourd'hui suite aux nombreuses réorganisations. A ce jour le gestionnaire de
layouts est démarré en même temps que l'ApplicationController et il est
arrêté conjointement, c'est à dire à la fermeture de l'application. De fait
il devrait toujours être démarré en dehors des phase de démarrage et de clôture.
<!-- Ignore Spelling: enregion -->

![DisplayController](./PlantUML/02-Architecture-01-Core-05-LayoutManager-01.png)

<h3>La spécialisation par système</h3>

<!-- Ignore Spelling: Guid -->
Afin de faciliter la gestion des layouts dont l'implémentation est souvent très
spécifique au système, un objet de type *OsLayoutId* a été mis en place, 
de type *String*: cet objet orienté plutôt système n'est pas explicité dans
l'interface *ILayoutManager*, mais il est utilisé dans l'implémentation de la
classe abstraite *LayoutManager*. Il pourrait éventuellement être renommé en 
*LayoutIdProxy* afin d'expliciter sa quasi redondance avec *LayoutId*, bien que
son mécanisme de Proxy n'ait pas été explicité à ce jour...

Par la suite l'implémentation des *LayoutManager* est spécialisée par dérivation des 
deux sous classes *WinLayoutManager* et *MacLayoutManager* respectivement.
Conjointement, la classe OsLayoutId est déclinée en *WinOsLayoutId* et 
*MacOsLayoutId*.
<!-- Ignore Spelling: enregion -->

![DisplayController](./PlantUML/02-Architecture-01-Core-05-LayoutManager-02.png)

<h2>Le gestionnaire de connexion</h2>

<h3>Principe</h3>

Le seul rôle du gestionnaire de connexion est de tenter de se connecter à un 
clavier identifié comme clavier Nemeio, avec priorité donnée à la connexion USB,
puis BLE s'il n'y a pas de clavier USB.

Ce gestionnaire doit également pouvoir signaler des erreurs de connexion
dans le cas où le clavier ne répondrait pas correctement à la tentative
de connexion.

Si le mécanisme est unifié quand au gestionnaire lui même - nous avons ici omis
l'interface car elle ne donne pas lieu à des instanciations distinctes selon
le système - nous verrons que les mécanisme de détection d'un clavier en USB
ou BLE seront spécifique à chaque système.

<h3>La classe <i>ConnectionManager</i></h3>

La classe ***ConnectionManager*** est démarrée au démarrage de l'application et
suspendue à chaque verrouillage de session. Elle sera par la suite relancée
au déverrouillage de session. Ce mécanisme assurer que le verrouillage de
session ne bloque pas le clavier et notamment que le changement d'utilisateur
sur un même système est possible.

Cette classe se base sur une instance de ***KeyboardConnectionSelector*** qui
assure la stratégie de priorisation des claviers disponibles: USB d'abord, 
BLE ensuite.
Cette classe contient également le mécanisme de bannissement d'un clavier qui
ne répondrait pas de façon systématique.
Le bannissement est opéré jusqu'à la reconnexion du clavier.

Le *KeyboardConnectionSelector* utilise lui même une 
***KeyboardConnectionWatcher*** qui assure le suivi sur le système des tous
les périphériques USB et BLE qui correspondent au profil Nemeio. Il utilise pour
cela deux sous instance de ***KeyboardListWatcher*** qui sont spécialisées:
- selon le type de périphérique filtré: USB ou BLE
- selon le système *Windows* ou *macOS*

![DisplayController](./PlantUML/02-Architecture-01-Core-06-ConnectionManager-01.png)

<h3>La spécialisation par système</h3>

Selon le système visé, le mécanisme de détection de périphérique est 
spécialisé selon le type de périphérique visé et au moyen de la technologie
disponible pour la plateforme

![DisplayController](./PlantUML/02-Architecture-01-Core-06-ConnectionManager-02.png)

<!-- Ignore Spelling: start, shutdown, debug -->
<!-- Ignore Spelling: enregion -->

