package main

import (
	"fmt"
	"io/ioutil"
	"math/rand"
	"os"
	"strings"
	"time"
)

type deck []string //like public class Deck : string[]{ ... }

func newDeck() deck {
	cards := deck{} //crate an empty array of string

	suits := []string{"Spades", "Diamonds", "Hearts", "Clubs"}
	values := []string{"Ace", "One", "Two"}

	for _, suit := range suits { //_ means we dont care about it
		for _, value := range values {
			cards = append(cards, suit+" of "+value)
		}
	}

	return cards
}

func (d deck) printIt() { //(d deck) is a receiver, likewise to create an extension method
	for i, card := range d {
		fmt.Println(i, "=>", card)
	}
}

func deal(d deck, size int) (deck, deck) { //Multiple return
	return d[:size], d[size:]
}

func (d deck) toString() string {
	return strings.Join([]string(d), ",")
}

func (d deck) saveToFile(fileName string) error {
	return ioutil.WriteFile(fileName, []byte(d.toString()), 0666)
}

func (d deck) readFromFile(fileName string) deck {
	bs, err := ioutil.ReadFile(fileName)
	if err != nil {
		fmt.Println("deck/readFromFile/Error:", err)
		os.Exit(1)
	}
	return deck(strings.Split(string(bs), ","))
}

func (d deck) shuffle() {
	for i := range d {
		newPosition := rand.Intn(len(d) - 1)
		d[i], d[newPosition] = d[newPosition], d[i] //One line swap
	}
}

func (d deck) shuffleBetter() {
	source := rand.NewSource(time.Now().UnixNano()) //Better random generation with current timestamp
	r := rand.New(source)

	for i := range d {
		newPosition := r.Intn(len(d) - 1)
		d[i], d[newPosition] = d[newPosition], d[i] //One line swap
	}
}
