---
title: "Hash Code 2015 Final Round"
summary: "Sujet extrait du PDF final_round_fr.pdf."
pdfUrl: "https://github.com/evilz/coding-challenges/blob/main/challenges/hashcode-2015/Src/FinalRound/Hashcode2015.FinalRound.App/Input/final_round_fr.pdf"
documentType: pdf
topics:
  - challenge
  - pdf
---

# Hash Code 2015 Final Round

[Open original PDF](https://github.com/evilz/coding-challenges/blob/main/challenges/hashcode-2015/Src/FinalRound/Hashcode2015.FinalRound.App/Input/final_round_fr.pdf)

Source PDF: `challenges/hashcode-2015/Src/FinalRound/Hashcode2015.FinalRound.App/Input/final_round_fr.pdf`

## Page 1

FR

Sujet de la finale, 28 mars 2015
Loon

Introduction
Tâche
Description du problème
La terre
Le vent
Décollage des ballons et changement d’altitude
Accès à Internet
Objectif
Données d’entrée
Exemple
Soumissions
Format de fichier
Exemple
Validation
Score

## Page 2

Introduction
Parfois, « tout le monde » ce n’est pas ​vraiment ​tout le monde. Par exemplelorsquel’ondit que
tout le monde a maintenant accèsàInternet. Enréalité, pour chaquepersonnesur leweb, il yenadeux
qui n’y sont pas.

Le​Projet Loonviseàfournir unaccèsuniversel àInternet grâceàuneflottedeballonsdehautealtitude
équipés d’émetteurs 4G. Ces ballons tournent autour du globe et permettent un accès à Internet dans
des zones inaccessibles par des moyens conventionnels.

Cependant, les ballons du projet Loon ne peuvent se ​déplacer seuls : chaque ballon peut ajuster son
altitude mais dépend des vents pour se déplacer. Ceci nécessite un contrôle précis des ballons pour
qu’ils se trouvent chacun à une altitude où des courants les enverront vers des zones habitées visées
par le projet.
Tâche
À partir des données météorologiques à différentes altitudes, vous devez planifier les changements
d’altitude d’une flotte de ballons pour fournir un accès à Internet à des endroits précis.
Description du problème
La terre
Les ballons volent au dessus d’une section de la terre délimitée par deux parallèles.

Exemple d’une section de la surface de la terre, délimitée par deux parallèles.

Cette section est représentée par une grille rectangulaire en deux dimensions, de cotés ​R et ​C​. Les
cases de cette grille sont repérées par une paire de coordonnées dont les index démarrent à 0,                      r, c][
dénotant respectivement la ligne (​row​) et la colonne (​column​) de la case.

Comme la terre est ronde, la grille boucleauxextrémitésdeslignes. C’est­à­direquechaquecase                                r, ][ 0
est voisine des cases   et  . La grille ne boucle ​pas ​aux extrémités des colonnes.r, ][ C−1 r, ][ 1

Un exemple de grille avec la case [0,0] dans le coin supérieur gauche. Deux cases sont marquées en noir et les
cases avec lesquelles elles sont voisines en vert et bleu.
Le vent
Dans ce problème, on suppose que les vents sont stables et ne changent pas avec le temps.

## Page 3

L’impact du vent sur les ballons est représenté par une grille de vecteurs ou et sont des                          a, )( b     a    b
entiers relatifs. Cette ​grille des mouvements correspondàlagrilledelasurfacedelaterre : unvecteur
 dans la case  de la grille des mouvements indique qu’un ballon positionné au dessus de  :a, )( b r, ][ c r, ][ c

● va se déplacer à la case , si (c’est­à­dire si la case dedestination            r , c ) % C][ +a ( +b    0≤r+a<R
est dans la section de la terre que nous considérons) ;
● sera irrémédiablement perdu et retiré de la simulation si   ou  .r+a<0 r+a≥R

Il y a ​Adifférentes grilles de mouvements : une pour chaque altitude considérée dans la simulation. On
suppose que le vent ne modifie jamais l’altitude d’un ballon.

Une colonne d’une grille des mouvements représentée à l’aide de flèches.

Décollage des ballons et changement d’altitude
Au démarrage de la simulation, tous les ballons sont dans la même ​case de départ​, au sol et prêts à
décoller.

La simulation fonctionne par tour. Au début de chaque tour :
● les ballons qui sont déjà en vol peuvent ajuster leur altitude de ­1, 0 ou 1, à condition que leur
altitude reste comprise entre 1 et ​A​. Un ballon qui a décollé ne peut plusjamaisseposer durant
la simulation ;
● lesballonsqui sont encoreausol peuvent décoller (enaugmentant leur altitudede1) ourester au
sol (en appliquant une modification de 0).

Après les changements d’altitude, tous les ballons en vol se déplacent de la valeur du vecteur
correspondant à leur position dans la grille des mouvements de leurs nouvelles altitudes (y compris les
ballons qui viennent d’être lancés à ce tour, mais pas ceux qui ont été perdus et retirés de la simulation).

Plusieurs ballons peuvent se trouver dans la même case et à la même altitude au même moment. En
particulier, il est possible de faire décoller plusieurs ballons par tour.
Accès à Internet
Àpartir dumoment oùilssont lancéset jusqu’àcequ’ilssoient perduset retirésdelasimulation, chaque
ballon fournit un accès à Internet aux cases se trouvant dans une région circulaire de rayon ​Vcentrée
sur le ballon. Pour un ballon positionné au dessus d’une case , la case est couverte par                      r, ][ c       u, ][ v
l’accès à Internet fournit par le ballon si et seulement si  .r ) columndist(c, ))( −u 2+( v 2 ≤V2

Où est une distance entre les colonnes qui prend en compte  olumndist(c , )  in(|c |, c |)c 1 c2 =m 1−c2 C−| 1−c2
le fait que la grille est circulaire aux extrémités des lignes.

## Page 4

Cases couvertes par un ballon positionné au centre pour V égal à 3.

L’altitude d’un ballon n’a pas d’impact sur la couverture qu’il fournit (on suppose que la différence de
couverture entre les altitudes est négligeable).
Objectif
Un ensemble de cases de la grille est désigné comme cible. À la fin de chaque tour (après que les
ballons aient été déplacés), chaque ​case cible couverte par au moinsunballoncontribuepour unpoint
au score final. L’objectif est d’obtenir le score le plus haut possible.
Données d’entrée
Les données d’entrée sont fournies dans un fichier texte contenant exclusivement descaractèresASCII
avec des lignes terminées par le caractère ‘\n’ (fins de ligne UNIX).

Le fichier est constitué ainsi :
● une ligne contenant les trois entiers naturels suivants, séparés par des espaces :
○ R​   ​: le nombre de lignes de la grille,1 000)( ≤R≤1
○ C​   : le nombre de colonnes de la grille,1 000)( ≤C≤1
○ A​   : le nombre d’altitudes différentes de la simulation ;1 000)( ≤A≤1
● une ligne contenant les quatre entiers naturels suivants, séparés par des espaces :
○ L​   : le nombre de cases cibles,1 000)( ≤L≤1
○ V ​(  : le rayon de la couverture fournie par les ballons,00)0≤V ≤1
○ B​  : le nombre de ballons disponibles,1 000)( ≤B≤1
○ T​  : le nombre de tour de la simulation ;1 000)( ≤T ≤1
● une ligne contenant une paire d’entiers naturels ​r​s et ​c​s séparés par une                    0 , )( ≤rs <R 0≤cs <C
espace, donnant la case de départ des ballons   ;r , ][ s cs
● L​ lignes décrivant les coordonnées des cases cibles, chaque ligne contenant :
○ une paire d’entiers naturels ​r​i et ​c​i donnant lescoordonnéesdelai​ème              0 , )( ≤ri <R 0≤ci <C
case cible . Chaque cible est différente desautres, maislacasededépart peut­être    r, ][ i ci
l’une des cases cibles.
● A sections décrivant les grilles des mouvements à chaque altitude allant de 1 à ​A​. Chaque
section est constituée de :
○ R​ lignes décrivant successivement les lignes de vecteurs. Chaque ligne contient :
■ Cpairesd’entiersnaturels​Δr​rc et ​Δc​rc séparés              − 00 r 00,− 00 c 00)( 1 ≤Δ rc ≤1 1 ≤Δ rc ≤1
par des espaces (aussi bien entre les paires qu’entre les entiers d’une paire).
Chaque paire décrit le vecteur de mouvement pour la case comme étant                    r, ][ c
.Δr , c )( rc Δ rc

## Page 5

Exemple
Voici un exemple de fichier d’entrée:
3 5 3
2 1 1 5
1 2
0 2
0 4
0 1 0 1 0 1 0 1 0 1
0 1 0 1 0 1 0 1 0 1
0 1 0 1 0 1 0 1 0 1
-1 0 -1 0 -1 0 -1 0 -1 0
-1 0 -1 0 -1 0 -1 0 -1 0
-1 0 -1 0 -1 0 -1 0 -1 0
0 1 0 1 0 1 0 2 0 1
0 2 0 1 0 2 0 3 0 2
0 1 0 1 0 1 0 2 0 1
3 lignes, 5 colonnes, 3 altitudes.
2 cibles, rayon de couverture de 1, 1 ballon, 5 tours.
Case de départ au centre de la carte (ligne 1, colonne 2).
Case cible au centre de la ligne 0.
Case cible à l’extrémité de la ligne 0.
Les vents à l’altitude 1 vont seulement à l’est.
..
..
Les vents à l’altitude 2 vont seulement au nord.
..
..
Les vents à l’altitude 3 vont seulement à l’est.
Par endroits, ils sont plus fort que ceux à l’altitude 1.
..
Exemple de fichier d’entrée.
Soumissions
Format de fichier
Un fichier de soumission doit être un fichier texte brut contenant seulement des caractères ASCII, avec
des lignes terminées soit par le seul caractère '\n' (style UNIX) ou les caractères '\r\n' (style Windows).

Le fichier doit contenir ​T lignes décrivant les changements d’altitude appliqués aux ballons à chaque
tour. Ces lignes doivent toutes contenir exactement ​B entiers égaux à ­1, 0 ou 1 et séparés par des
espaces, décrivant les changements d’altitude appliqués à chaque ballon à un tour donné de la
simulation.

Il est nécessaire de fournir ​B changements d’altitude à chaque tour, y compris si des ballons ont été
perdus et retirés de la simulation. Les changements appliqués aux ballons perdus sont sans effet (les
ballons ne reviendront jamais), mais ils doivent quand même se conformer aux spécifications
(c’est­à­dire qu’ils doivent être 1, 0 ou ­1 et ils ne doivent pas faire passer l’altitudeduballonaudessus
de ​A​ ou en dessous de 1).

Exemple
Le fichier de soumission suivant correspond à l'exemple de fichier d'entrée donné ci­dessus.
1
1
1
0
0
Décollage du ballon dès le premier tour.
Le ballon atteint l’altitude 2 à la fin du deuxième tour.
Le ballon atteint l’altitude 3.
Le ballon reste à l’altitude 3.
Le ballon reste à l’altitude 3.
Exemple de fichier de soumission.

tour 2  tour 3  tour 4  tour 1
    départ  tour 0

Position du ballon à la fin de chaque tour, les cases cibles sont marquées en jaune.

## Page 6

Cet exemple de soumissions obtiendrait le score suivant :
● 2 points à la fin du tour 1, quand les deux cibles sont couvertes par le ballon,
● 1 points à la fin du tour 2, quand la seconde cible est couverte,
● 1 points à la fin du tour 3, quand la première cible est couverte,
● 1 points à la fin du tour 5, quand la première cible est couverte.

Cette soumission marquerait donc un total de 5 points. Aucun point n’est obtenu à la fin du tour 0, car
aucune cible n’est couverte.
Validation
Pour être acceptée, une solution doit satisfaire les critères suivants :
● le format du fichier doit correspondre à la description ci­dessus,
● la somme des changements d’altitude pour un ballon, entre le début de la simulation et un tour
donné doit :
○ rester à 0 jusqu’au décollage du ballon (il n’est pas autorisé de faire descendre unballon
avant qu’il ne soit lancé),
○ rester dans l'intervalle [1, ​A​] (inclus) une fois que le ballon a décollé.
Score
Le ​temps de couverture pour une case cible est le nombre total detoursàlafindesquelslacaseétait
couverte par au moins un ballon (après le mouvement des ballons).
Le score final est la somme des temps de couverture pour toutes les cases cibles.
