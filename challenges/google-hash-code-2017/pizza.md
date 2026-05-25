---
title: "Google Hash Code 2017 Pizza"
summary: "Sujet extrait du PDF pizza.pdf."
pdfUrl: "https://github.com/evilz/coding-challenges/blob/main/challenges/google-hash-code-2017/pizza.pdf"
documentType: pdf
topics:
  - challenge
  - pdf
---

# Google Hash Code 2017 Pizza

[Open original PDF](https://github.com/evilz/coding-challenges/blob/main/challenges/google-hash-code-2017/pizza.pdf)

Source PDF: `challenges/google-hash-code-2017/pizza.pdf`

## Page 1

Pizza
Practice   Problem   for   Hash   Code   2017
Introduction
Didyouknowthatatanygiventime,someoneiscuttingpizzasomewherearoundtheworld?Thedecision
abouthowtocutthepizzasometimesiseasy,butsometimesit’s reallyhard:youwantjusttherightamount
of   tomatoes   and   mushrooms   on   each   slice.   If   only   there   was   a   way   to   solve   this   problem   using   technology...
Problem   description
Pizza
Thepizzaisrepresentedasarectangular,2-dimensionalgridof R rowsand Ccolumns.Thecellswithinthe
gridarereferencedusinga pairof 0-basedcoordinates , denotingrespectivelytherowandthe         r, c][
column   of   the   cell.

Each   cell   of   the   pizza   contains   either:
● mushroom,   represented   in   the   input   file   as    M ;   or
● tomato,   represented   in   the   input   file   as    T
Slice
Asliceofpizzaisarectangularsectionofthepizzadelimitedbytworowsandtwocolumns,withoutholes.
Thesliceswewanttocutoutmustcontainatleast L cellsofeachingredient(thatis,atleast Lcellsof
mushroomandatleast Lcellsoftomato)andatmost Hcellsofanykindintotal-surprisingasitis,thereis
such   a   thing   as   too   much   pizza   in   one   slice.

The   slices   being   cut   out   cannot   overlap.   The   slices   being   cut   do   not   need   to   cover   the   entire   pizza.
Goal
The   goal   is   to   cut   correct   slices   out   of   the   pizza   maximizing   the   total   number   of   cells   in   all   slices.
Input   data   set
Theinputdataisprovidedasadatasetfile- aplaintextfilecontainingexclusivelyASCIIcharacterswith
lines   terminated   with   a   single   ‘\n’   character   at   the   end   of   each   line   (UNIX- style   line   endings).
File   format
The   file   consists   of:
● one   line   containing   the   following   natural   numbers   separated   by   single   spaces:
○ R        is   the   number   of   rows,1 000)( ≤R≤1
○ C       is   the   number   of   columns,1 000)( ≤C≤1
○ L        is   the   minimum   number   of   each   ingredient   cells   in   a   slice,1 000)( ≤L≤1
○ H        is   the   maximum   total   number   of   cells   of   a   slice1 000)( ≤H≤1

©   Google   2017,   All   rights   reserved.

## Page 2

● R linesdescribingtherowsof thepizza(oneafteranother).Eachof theselinescontains C
charactersdescribingtheingredientsinthecellsoftherow(onecellafteranother).Eachcharacter
is   either   ‘M’   (for   mushroom)   or   ‘T’   (for   tomato).
Example

3   5   1   6
TTTTT
TMMMT
TTTTT
3   rows,   5   columns,   min   1   of   each   ingredient   per   slice,   max   6   cells   per   slice
Example   input   file.
Submissions
File   format
The   file   must   consist   of:
● onelinecontaininga singlenaturalnumber S , representingthetotalnumberof        0 )( ≤S≤R×C
slices   to   be   cut,
● U linesdescribingtheslices.Eachof theselinesmustcontainthefollowingnaturalnumbers
separated   by   single   spaces:
○ r 1 ,  c 1 ,  r 2 ,  c 2 describeasliceofpizzadelimitedbytherows r 1and    0 , , , )( ≤r1 r2 <R 0≤c1 c2 <C
r 2 andthecolumns c 1 and c 2 , includingthecellsofthedelimitingrowsandcolumns.The
rows( r 1 and r 2 )canbegiveninanyorder.Thecolumns( c 1and c 2 )canbegiveninanyorder
too.
Example
3
0   0   2   1
0   2   2   2
0   3   2   4
3   slices.
First   slice   between   rows   (0,2)   and   columns   (0,1).
Second   slice   between   rows   (0,2)   and   columns   (2,2).
Third   slice   between   rows   (0,2)   and   columns   (3,4).
Example   submission   file.

©   Google   2017,   All   rights   reserved.

## Page 3

Slices   described   in   the   example   submission   file   marked   in   green,   orange   and   purple.
Validation
For   the   solution   to   be   accepted:
● the   format   of   the   file   must   match   the   description   above,
● each   cell   of   the   pizza   must   be   included   in   at   most   one   slice,
● each   slice   must   contain   at   least    L    cells   of   mushroom,
● each   slice   must   contain   at   least    L    cells   of   tomato,
● total   area   of   each   slice   must   be   at   most    H
Scoring
The   submission   gets   a   score   equal   to   the   total   number   of   cells   in   all   slices.

Notethattherearemultipledatasetsrepresentingseparateinstancesoftheproblem.Thefinal
score   for   your   team   is   the   sum   of   your   best   scores   on   the   individual   data   sets.
Scoring   example

The   example   submission   file   given   above   cuts   the   slices   of   6,   3   and   6   cells,   earning   6   +   3   +   6   =   15   points.

©   Google   2017,   All   rights   reserved.
