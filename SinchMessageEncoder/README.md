# Simple message encoder

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

A message is not required to have any headers or payload data to be valid, but must contain the payload delimieter.
The following is therefore a valid message
```
SOP
```