package main

import (
	"os"
	"testing"
)

func Test_NewDeck(tester *testing.T) {
	d := newDeck()
	if len(d) != 12 {
		tester.Errorf("Expecting 12 but got %v", len(d))
	}

	if d[0] != "Spades of Ace" {
		tester.Errorf("Expecting Spades of Ace but got %v", d[0])
	}
}

func Test_SaveToFile(tester *testing.T) {
	os.Remove("_decktesting")

	deck := newDeck()
	deck.saveToFile("_decktesting")

	loadedDeck := deck.readFromFile("_decktesting")

	if len(loadedDeck) != 16 {
		tester.Errorf("Expecting 16 but got %v", len(loadedDeck))
	}

	os.Remove("_decktesting")
}
