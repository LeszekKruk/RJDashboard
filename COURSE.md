# Tworzenie nagłówków h1
## Nagłówek h2
### Nagłówek h3
#### Nagłówek h4
##### Nagłówek h5
###### Nagłówek h6

# Formatowanie tekstu
Bold **tekst pogrubiony** lub __tekst pogrubiony__

Italic *tekst pochylony* lub _tekst pochylony_

Strikethrough ~~tekst przekreślony~~

# Cytaty
Konfucjusz
> "Jeśli kochasz to, co robisz, to nie jest to praca"

# Komentowanie kodu
Użyj `git status` aby uzyskać efekt Bold

# Linki
Adres mojego bloga to [Leszek Kruk](http://www.krukcom.pl).

## Linki do sekcji
You can link directly to a section in a rendered file by hovering over the section heading to expose the link
require "Cytaty"

## Linki względne
You can define relative links and image paths in your rendered files to help readers navigate to other files in your repository.

A relative link is a link that is relative to the current file. For example, if you have a README file in root of your repository, 
and you have another file in docs/CONTRIBUTING.md, the relative link to CONTRIBUTING.md in your README might look like this:

[Link względny do pliku w tym samym katalogu](README.md)

GitHub will automatically transform your relative link or image path based on whatever branch you're currently on, so that the link or path always works. You can use all relative link operands, such as ./ and ../.

Relative links are easier for users who clone your repository. Absolute links may not work in clones of your repository - we recommend using relative links to refer to other files within your repository.

## URL
GitHub automatycznie tworzy link ze standardowego adresu URL.

Mój blog http://krukcom.pl

[I'm an inline-style link](https://www.google.com)

[I'm an inline-style link with title](https://www.google.com "Google's Homepage")

[I'm a reference-style link][Arbitrary case-insensitive reference text]

[I'm a relative reference to a repository file](../blob/master/LICENSE)

[You can use numbers for reference-style link definitions][1]

Or leave it empty and use the [link text itself].

URLs and URLs in angle brackets will automatically get turned into links. 
http://www.example.com or <http://www.example.com> and sometimes 
example.com (but not on Github, for example).

Some text to show that the reference links can follow later.

[arbitrary case-insensitive reference text]: https://www.mozilla.org
[1]: http://slashdot.org
[link text itself]: http://www.reddit.com

## YouTube 
Youtube videos

They can't be added directly but you can add an image with a link to the video like this:

<a href="http://www.youtube.com/watch?feature=player_embedded&v=YOUTUBE_VIDEO_ID_HERE
" target="_blank"><img src="http://img.youtube.com/vi/YOUTUBE_VIDEO_ID_HERE/0.jpg" 
alt="IMAGE ALT TEXT HERE" width="240" height="180" border="10" /></a>
Or, in pure Markdown, but losing the image sizing and border:

[![IMAGE ALT TEXT HERE](http://img.youtube.com/vi/YOUTUBE_VIDEO_ID_HERE/0.jpg)](http://www.youtube.com/watch?v=YOUTUBE_VIDEO_ID_HERE)
Referencing a bug by #bugID in your git commit links it to the slip. For example #1.

# Listy

- Lista 1
- Lista 2
- Lista 3

* Lista 1
* Lista 2
* Lista 3

1. Lista 1
2. Lista 2
3. Lista 3

# Listy zagnieżdżone - tworzymy poprzez użycie podwójnej spacji

1. Lista 1
  * 1.1. aaa
  * 1.2. bbb
2. Lista 2
  * ccc
    * ddd
3. Lista 3

# Listy zadań
- [x] @mentions, #refs, [links](), **formatting**, and <del>tags</del> supported
- [ ] Zadanie 1
- [ ] \(Optional) Zadanie 2
- [x] Zadanie 3

If a task list item description begins with a parenthesis, you'll need to escape it with \:

# Mentioning
@LeszekKruk/krukcom Co myślisz o tym tutorialu?

# Odwoływanie się do problemów i pull requests
You can bring up a list of suggested Issues and Pull Requests within the repository by typing #. Type the issue or PR number or title to filter the list, then hit either tab or enter to complete the highlighted result.

# Używanie emotionów
Możemy używać emotionów wpisując :KODEMOTION'a:.

@LeszekKruk :+1:

# Ustępy i podziały wierszy
Nowy akapit można utworzyć, pozostawiając pustą linię między wierszami tekstu.

# Ignorowanie formatowania 
Możesz powiedzieć GitHub o ignorowaniu (lub ucieczce) formatowaniu Markdown, używając \ przed znakiem Markdown.

Ten tekst nie będzie \*pogrubiony\*.

# Tworzenie tabel
Można tworzyć tabele z rurami i łączniki -. Dzielnice są używane do tworzenia nagłówków każdej kolumny, a rury oddzielają każdą kolumnę. Przed tabelą należy wstawić pustą linię, aby poprawnie ją renderować.

| Nagłówek 1 | Nagłówek 2 |
|------------|------------|
| Komórka 1  | Komórka 2  |
| Komórka 3  | Komórka 4  |

Rury na obu końcach stołu są opcjonalne.

Komórki mogą różnić się szerokością i nie muszą być idealnie ustawione w kolumnach. W każdej kolumnie wiersza nagłówka musi znajdować się co najmniej trzy łączniki.

## Formatowanie zawartości tabeli
Możemy używać wcześniej zaprezentowanych stylów formatujących jak: linki, stylizacje tekstu, bloki kodu

Do wyrównywania tekstu w komórkach możemy użyć kodu:
* :--- - wyrównanie do lewej
* :---: - wycentrowanie
* ---: - wyrównanie do prawej

# Blokowanie bloków kodu
Możemy utworzyć blok kodu poprzez umieszczenie potrójnych ``` przed i po kodzie.

```
 function mdTutorial(){
     
 }
```

# Kolorowanie składni
Możesz dodać dowolny identyfikator języka, aby umożliwić podświetlanie składni w zablokowanym bloku kodu.

Na przykład, aby podświetlić składnię kod Ruby:

```ruby
require 'redcarpet'
markdown = Redcarpet.new("Hello World!")
puts markdown.to_html
```

# Grafiki
![GitHub Logo](/images/logo.png)
Format: ![Alt Text](http://krukcom.pl "Opcjonalny atrybut")

# Poziome linie

* * *

***

- - 
