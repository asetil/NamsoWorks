package utils

import (
	"fmt"

	"gopkg.in/guregu/null.v3"
)

func parseDate(date null.Time) string {
	if date.Valid {
		year, month, day := date.Time.Date()
		return fmt.Sprintf("%02d.%02d.%d", day, month, year)
	}

	return ""
}
