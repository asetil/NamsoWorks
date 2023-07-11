package utils

import (
	"fmt"
	"math"
)

type Packet []byte

func (p *Packet) SetLength(length int16) {
	n := length / 256
	(*p)[3] = byte(n)
	(*p)[2] = byte(length - n*256)
}

func (p *Packet) Insert(data []byte, i int) {
	_data := make([]byte, len(data))
	copy(_data, data)
	*p = append((*p)[:i], append(_data, (*p)[i:]...)...)
}

func (p *Packet) Overwrite(data []byte, i int) {
	_data := make([]byte, len(data))
	copy(_data, data)
	*p = append((*p)[:i], append(_data, (*p)[i+len(data):]...)...)
}

func (p *Packet) Concat(data []byte) {
	p.Insert(data, len(*p))
}

func (p *Packet) Print() {
	var print string
	for _, b := range *p {
		print += fmt.Sprintf("%02X ", b)
	}
	fmt.Printf("%s\n", print)
}

func FloatToBytes(dec float64, bytes byte, reverse bool) []byte {
	return IntToBytes(uint64(math.Float32bits(float32(dec))), bytes, reverse)
}

func IntToBytes(dec uint64, bytes byte, reverse bool) []byte {

	arr := make([]byte, 8)
	if dec == 0 {
		return arr[:bytes]
	}

	if bytes < 8 {
		dec = dec % (1 << (bytes * 8))
	}

	for i := 7; i >= 0; i-- {
		power := uint64(math.Pow(256, float64(i)))
		arr[i] = byte(dec / power)
		if arr[i] > 0 {
			dec -= uint64(arr[i]) * power
		}
	}

	if reverse {
		return arr[:bytes]
	}

	return reverseBytes(arr)[8-bytes : 8]
}

func reverseBytes(input []byte) []byte {
	if len(input) == 0 {
		return input
	}

	return append(reverseBytes(input[1:]), input[0])
}
