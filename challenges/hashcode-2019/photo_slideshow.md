---
title: "Photo Slideshow"
summary: "Sujet extrait du PDF photo_slideshow.pdf."
pdfUrl: "https://github.com/evilz/coding-challenges/blob/main/challenges/hashcode-2019/photo_slideshow.pdf"
documentType: pdf
topics:
  - challenge
  - pdf
---

# Photo Slideshow

[Open original PDF](https://github.com/evilz/coding-challenges/blob/main/challenges/hashcode-2019/photo_slideshow.pdf)

Source PDF: `challenges/hashcode-2019/photo_slideshow.pdf`

## Page 1

Photo slideshow
Problem statement for the Online Qualication Round of Hash Code 2019

## Page 2

Introduction
As the saying goes, "a picture is woh a thousand words."  We agree – photos are an
impoant pa of contemporary digital and cultural life. Approximately 2.5 billion 1
people around the world carry a camera – in the form of a smaphone – in their
pocket every day. We tend to make good use of it, too, taking more photos than ever
(back in 2017, Google Photos announced it was backing up more than 1.2 billion photos
and videos per day).  2

The rise of digital photography creates an interesting challenge: what should we do
with all of these photos? In this competition problem, we will explore the idea of
composing a slideshow out of a photo collection.
Task
Given a list of photos and the tags associated with each photo, arrange the photos into
a slideshow that is as interesting as possible (the scoring section below explains what
we mean by “interesting”).
Problem description
Photos
A photo is described by a set of tags.

For  example , a photo with a cat on a beach, during a sunny aernoon could be
tagged with the following tags: [cat, beach, sun].

1 hps://www.statista.com/statistics/330695/number-of-smaphone-users-worldwide/
2 hps://www.blog.google/products/photos/google-photos-500-million-new-sharing/

## Page 3

Each photo's orientation is either horizontal or veical.

the photo on the le is horizontal, while the photo on the right is veical
Slideshow
A slideshow is an ordered list of slides. Each slide contains either:
● a single horizontal photo, or
● two veical photos side-by-side

If the slide contains a single horizontal photo, the tags of the slide are the same as the
tags of the single photo it contains.

For  example , a slide containing a single horizontal photo with tags [cat, beach, sun],
has tags [cat, beach, sun].

If the slide contains two veical photos, the tags of the slide are all the tags present in
any or both of the two photos it contains.

For  example , a slide containing two veical photos with tags [sele, smile] for the
rst photo, and tags [garden, sele] for the second photo, has tags [sele, smile,
garden].

Each photo can be used either once or not at all. The slideshow must have  at least  one
slide.

## Page 4

Input data set
File format
Each input data set is provided in a plain text le containing exclusively ASCII
characters with lines terminated with a single '\n' character (UNIX- style line endings).

The rst line of the data set contains a single integer  N  ( 1 ≤ N ≤ 10 5 ) — the number of
photos in the collection.

This is followed by  N  lines, where line  i  contains a description of the photo with ID  i
(0 ≤  i  <  N ). The description of photo  i  contains the following data, separated by a single
space:
● A single character ‘H’ if the photo is horizontal, or ‘V’ if it is veical.
● An integer  M i   (1 ≤  M i  ≤ 100) — the number of tags for that photo.
● M i  text strings — the tags for photo  i . Each tag consists only of lowercase ASCII
leers and digits, between 1 and 10 characters in total.
Example

cat, beach, sun sele, smile garden, sele garden, cat

Input file  Description
4
H 3 cat beach sun
V 2 selfie smile
V 2 garden selfie
H 2 garden cat
The collection has 4 photos
Photo 0 is horizontal and has tags [cat, beach, sun]
Photo 1 is vertical and has tags [selfie, smile]
Photo 2 is vertical and has tags [garden, selfie]
Photo 3 is horizontal and has tags [garden, cat]

## Page 5

Submissions
File format
The output le must sta with a single integer  S  ( 1 ≤ S ≤ N  )— the number of slides in the
slideshow. This must be followed by  S  lines describing the individual slides. Each line
should contain either:
● A single integer – ID of the single horizontal photo in the slide.
● Two integers separated by a single space – IDs of the two veical photos in the
slide in any order.

Each photo can be used only one time or not at all.
Example

slide S 0 slide S 1 slide S 2

Submission file  Description
3
0
3
1 2
The slideshow has 3 slides
First slide contains photo 0
Second slide contains photo 3
Third slide contains photos 1 and 2

## Page 6

Scoring
The slideshow is scored based on how interesting the transitions between each pair of
subsequent (neighboring) slides are. We want the transitions to have something in
common to preserve continuity (the two slides should not be totally dierent), but we
also want them to be dierent enough to keep the audience interested. The similarity
of two veical photos on a single slide is not taken into account for the scoring
function. This means that two photos can, but don't have to, have tags in common.

For two subsequent slides  S i  and  S i+1 , the interest factor is the minimum (the smallest
number of the three) of:
● the number of common tags between  S i  and  S i+1
● the number of tags in  S i  but not in  S i+1
● the number of tags in  S i+1  but not in  S i .

For  example , for the slide transition from S 1  to S 2 , we know that the tags are [garden,
cat] for S 1 , and [sele, smile, garden] for S 2 :

● The number of common tags is 1 → [garden]
● The number of tags in S 1 , but not is S 2  is 1 → [cat]
● The number of tags in S 2 , but not in S 1 , is 2 →  [sele and smile]

The interest factor is the minimum of these numbers, so it is 1.

For a slideshow of S slides, the score will be equal to the sum of interest factors of
each transition of two neighboring slides. A slideshow with only one slide has a score
of zero.

## Page 7

For  example , with the input and the submission les above, the slideshow has 3
slides, hence it has 2 transitions:

1st transition, from slide S 0  (photo 0) to slide S 1  (photo 3)
● 1 common tag between photos 0 and 3 → [cat]
● 2 tags in photo 0 and not in photo 3 → [beach, sun]
● 1 tag in photo 3 and not in photo 0 →  [garden]

Interest factor = min(1, 2, 1) = 1

Second transition, from slide S 1  (photo 3) to slide S 2  (photos 1, 2) has interest factor 1
(see example above).

Therefore, the score of this submission is 1 + 1 = 2.

Note that therearemultipledatasetsrepresentingseparateinstancesof theproblem. Thenal
score for your team will be the sum of your best scores on the individual data sets.
