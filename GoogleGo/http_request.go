package main

import (
	"fmt"
	"io"
	"net/http"
	"os"
)

func doRequest(url string) (*http.Response, error) {
	res, err := http.Get(url)
	return res, err
}

func testHttp() {
	res, err := doRequest("https://www.yenibiris.com")
	if err != nil {
		fmt.Println("Error:", err)
		os.Exit(1)
	}
	fmt.Println("Response :", res)

	p := make([]byte, 99999)
	res.Body.Read(p)
	//fmt.Println("Response Body:", string(p))

	//Veya
	io.Copy(os.Stdout, res.Body)
}
