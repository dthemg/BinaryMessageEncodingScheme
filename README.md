# Simple message encoder


## Design
This message encoding scheme uses three non-ASCII characters to delimit the encoded byte message into recognizable sections.

* One key-value-delimiter character, below denoted `KVD`, to delimit a header key from its value
* One end-of-header character, below denoted `EOH`, to delimit the ending of a header
* One character, below denoted `SOP`, for declaring the start of the payload

Encoding a message with the following data
```
headers: {
    "ab": "cd",
    "ef": "gh"
}
payload: [ 129, 1, 231]
```

Will result in the following sequence
```
a b KVD c d EOH e f KVD g h EOH SOP 129 1 231
```

## Design choices

* Implemented as separate classes to adhere to SRP
* Focused on tests, error handling and keeping the code clean over performance
* Complexity-wise the parsing should scale O(n) with the size of the message
