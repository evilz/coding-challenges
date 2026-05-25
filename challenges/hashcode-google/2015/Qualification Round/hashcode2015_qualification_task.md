---
title: "Hash Code 2015 Qualification Round"
summary: "Sujet extrait du PDF hashcode2015_qualification_task.pdf."
pdfUrl: "https://github.com/evilz/coding-challenges/blob/main/challenges/hashcode-google/2015/Qualification%20Round/hashcode2015_qualification_task.pdf"
documentType: pdf
topics:
  - challenge
  - pdf
---

# Hash Code 2015 Qualification Round

[Open original PDF](https://github.com/evilz/coding-challenges/blob/main/challenges/hashcode-google/2015/Qualification%20Round/hashcode2015_qualification_task.pdf)

Source PDF: `challenges/hashcode-google/2015/Qualification Round/hashcode2015_qualification_task.pdf`

## Page 1

EN

Problem statement for the Qualification Round, March 12th, 2015
Optimize a Data Center

Introduction
For over ten years, Google has been building data centers of its own design, deploying thousands of
machines in locations around the globe. In each of these locations, batteries of servers are at work
around the clock, running services we use every day, fromGoogle Search and YouTube to the judge
system of Hash Code.

Data center design is a multi­factor optimization problem. Surely, we want to get as much computing
capacity as possible —the services need a lot of resourcestodayandwill beaskingfor moretomorrow.
But maximizing the rawcapacity is not the only goal: it’s equally important to ensure that thecomputing
capacity is provided reliably even in the face of inevitable hardware failures.

Task
Servers in a data center are ​physically divided into rows​. Rows can share resources such aselectric
power. If such a shared resource fails, we assume that the entire rowis lost and all servers in that row
become ​unavailable​.

Servers in a data center are also​logicallydividedintopools. ​Eachserver belongstoexactlyonepool,
and provides it with some amount of computingresources, called​capacity​. Thecapacityof apool isthe
sum of the capacities of the ​available​ servers in that pool.

To ensure reliability of a pool, it is therefore desirable to distribute its servers between different rows.
Then, when a rowfails, the pool can continue to operate on the servers fromtheremainingrows(albeit

## Page 2

with a reduced capacity because of the unavailable servers). The ​guaranteed capacity ​of a pool isthe
minimum capacity it will have when at most one data center row goes down.

Given a schema of a data center and a list of available servers, your goal is to assign servers to ​slots
within the rows​ and to ​logical pools​ so that the ​lowest guaranteed capacity​ of all pools is maximized.
Problem description
Slots
A data center is modeled as ​rows​ of ​slots​ in which servers can be placed.

A data center with four rows highlighted in different colors.

Some of the slots might be unavailable (e.g. because of other installations occupying some of the slots).

Six unavailable slots of the data center marked with red crosses.
Servers
Each server is characterized by its ​size and ​capacity​. Size is thenumber of consecutiveslotsoccupied
by the machine. Capacity is the total amount of CPU resources of the machine (an integer value).

Servers of different sizes placed in two rows.
Input data
The input dataisprovidedinaplaintext filecontainingexclusivelyASCII characterswithlinesterminated
with a single ‘\n’ character at the end of each line (UNIX­style line endings).

The file consists of:

## Page 3

● one line containing the following five natural numbers separated by single spaces:
○ R​   ​denotes the number of rows in the data center,1 000)( ≤R≤1
○ S​   denotes the number of slots in each row of the data center,1 000)( ≤S ≤1
○ U​   denotes the number of unavailable slots,0 )( ≤U ≤R×S
○ P​   denotes the number of pools to be created,1 000)( ≤P ≤1
○ M​   denotes the number of servers to be allocated;1 )( ≤M≤R×S
● U subsequent lines describing the unavailable slots of the data center. Each of these lines
contains two natural numbers separated by a single space: ​r​i and ​s​i ( ,                        , )0≤ri <R 0≤si <S
denoting the rownumber (​r​i​) and the slot number within the row(​s​i​) of the particular unavailable
slot;
● M subsequent lines describing the servers to be allocated. Each of these lines contains two
natural numbersseparatedbyasinglespace: ​z​i and​c​i , denotingthesize                    1 , 000)( ≤zi ≤S 1≤ci ≤1
of the server (​z​i​), ie. the number of slots that it occupies, and the capacity (​c​i​) of the machine.
Example
An example input file could look as follows.
2 5 1 2 5
0 0
3 10
3 10
2 5
1 5
1 1
2 rows of 5 slots each, 1 slot unavailable, 2 pools and 5 servers.
Coordinates of the first and only unavailable slot.
First server takes three slots and has a capacity of 10.
So does the second one.
The third one takes two slots and has a capacity of 5.
The fourth one takes just one slot and has a capacity of 5.
The fifth one takes just one slot too and has a capacity of 1.
Example input file.

The file above describes a data center of two rows of five slots each and servers of different parameters.

D​ata center and servers described in the above example​.
Submissions
File format
Asubmission file has to be a plain text file containing exclusivelyASCII characterswithlinesterminated
with either a single ‘\n’ character at the end of each line (UNIX­style line endings) or ‘\r\n’ characters at
the end of each line (Windows­style line endings).

The file has to consists of ​Mlinesdescribingtheallocationof theindividual servers, inthesameorder as
they appeared in the input file. Each of these lines has to contain either:
● three natural numbers separated bysinglespaces: ​ar​i​, ​as​i​, ​ap​i                  0 r , s , )( ≤a i <R 0≤a i <S 0≤api <P
denoting the allocated row (​ar​i​) and slot within the row (​as​i​) for the server, and the allocated
logical pool for it (​ap​i​);
● the single lowercase “x” letter, if the server is left unallocated.

## Page 4

For servers that occupy more than one slot, the slot of the lowest index (leftmost in the picture below)
should be indicated as the position of the server.

The position of the three­slot server in the picture should be described as ar​i​ = 0 and as​i​ = 2.

Example
The following example submission file corresponds to the example input file presented above.
0 1 0
1 0 1
1 3 0
0 4 1
x
Server 0 placed in row 0 at slot 1 and assigned to pool 0.
Server 1 placed in row 1 at slot 0 and assigned to pool 1.
Server 2 placed in row 1 at slot 3 and assigned to pool 0.
Server 3 placed in row 0 at slot 4 and assigned to pool 1.
Server 4 not allocated.
Example submission file.

Server layout corresponding to the given example.
Validation
For the solution to be accepted, it has to meet the following criteria:
● the format of the file has to match the description above,
● each slot of the data center has to be occupied by at most one server,
● no server occupies any unavailable slot of the data center,
● no server extends beyond the slots of the row.
Score
For each pool, its ​guaranteed capacity is defined as the lowest total capacity of its running servers
when exactly one of the data center rows is unavailable (over all possible rows). The score awarded to
the submission is the ​lowest​ guaranteed capacity of all pools. The goal is to maximize this score.

## Page 5

Example
Intheexamplesubmissiongivenabove, thetotal scoreis​5​, astheguaranteedcapacitiesof bothpoolsis
5 and  . See the figure for details.in(5, )m 5 =5

  row 0  row 1  guaranteed capacity
pool 0  10  5  5
pool 1  5  10  5
Guaranteed capacities of each pool in the example submission.

Formally speaking
For ​i​ ( ), the ​guaranteed capacity​ ​gc​i​ can be defined as:0≤i <P
c  in  (   )   g i =m 0≤r<R ∑
M−1
k = 0, server k in pool i
ci −  ∑
M−1
k=0, server k in pool i, server k in row r
ci

and the total score can be defined as:

core  gc  s =min0≤i<P i
