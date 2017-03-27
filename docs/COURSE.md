# Nagłówek h1
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
Użyj `git status` aby uzyskać ...

# Linki
Adres mojego bloga to [Leszek Kruk](http://www.krukcom.pl).

## Linki do sekcji
Możesz umieszczać link bezpośrednio do sekcji umieszczonej nad nagłówkiem

## Linki względne
Możesz umieścić linki względne i ścieżki do grafik w wyświetlanych plikach aby pomóc czytelnikom przechodzić do innych plików umieszczonych w repozytorium.

Link względny jest linkiem, który związany jest z aktualnym plikiem. 
Np. jeżeli masz plik README w katalogu głównym repozytorium i chcesz umieścić link do pliku umieszczonego w docs/COURSE.md to link względny może wyglądać następująco:

[Link względny](LICENCE)

Linki względne są lepsze dla użytkowników, którzy klonują repozytoria. Linki bezwzględne mogą nie zadziałać w kopiach repozytorium.end using relative links to refer to other files within your repository.

## URL
GitHub automatycznie tworzy link ze standardowego adresu URL.

Mój blog http://krukcom.pl

[Link do bloga](https://krukcom.pl)

[Link do bloga z tytułem](https://www.google.com "Leszek Kruk - blog")

[Link względny do pliku w repozytorium](../docs/LICENSE)

[Możesz użyć numerów dla zdefiniowanych linków][1]

Albo pozostawić pusty link [opis linku].

[1]: http://krukcom.pl
[opis linku]: http://krukcom.pl

## YouTube - videos

Nie można dodawać ich bezpośrednio, ale możemy dodać obraz z linkiem do tego filmu:

<a href="http://www.youtube.com/link_do_filmu
" target="_blank"><img src="http://img.youtube.com/link_do_grafiki" 
alt="IMAGE ALT TEXT HERE" width="240" height="180" border="10" /></a>

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
- [ ] Zadanie 2
- [x] Zadanie 3

Jeżeli opis elementu listy zadań zaczyna się nawiasem, musimy poprzedzić go za pomocą \

# Mentioning
@LeszekKruk/krukcom Co myślisz o tym tutorialu?

# Odwoływanie się do problemów i pull requests
-

# Używanie emotionów
Możemy używać emotionów wpisując :KODEMOTION'a:.

@LeszekKruk :+1:

# Ustępy i podziały wierszy
Nowy akapit można utworzyć, pozostawiając pustą linię między wierszami tekstu.

# Ignorowanie formatowania 
Możesz poinformować GitHub o ignorowaniu (lub ucieczce) formatowania Markdown, używając \ przed znakiem Markdown.

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
