# UnityGoogleSpreadsheet

Get Google Spreadsheet data from Unity Editor.

## Requirement

* Unity Editor 2023.2.16f1 

It should work with other versions, but we haven't tested it.

## Installation

### Install via UPM (using Git URL)

1. Navigate to your project's Packages folder and open the manifest.json file.
2. Add this line in `dependencies`.
```
"dependencies": {
    "studio.shirokurohitsuji.unity-googlespreadsheet": "https://github.com/shirokurohitsuji/UnityGoogleSpreadsheet.git"
}
```
3. UPM should install the package.

## How to Use

1. See [Sample](https://github.com/shirokurohitsuji/UnityGoogleSpreadsheet/tree/main/~Sample) to create Editor Window.
2. Create Google App Script like below to get JSON data.
```typescript
function doGet(e) {
  if (!e.parameter.sheet) e.parameter.sheet = 'foo';
  const book = _getBook(e.parameter.book);
  const sheet = book.getSheetByName(e.parameter.sheet);
  const data = _getData(sheet);
  return ContentService.createTextOutput(JSON.stringify(data, null, 2) + "\n")
  .setMimeType(ContentService.MimeType.JSON);
}

function _getData(sheet) {
  const rows = sheet.getDataRange().getValues();
  const keys = rows.splice(0, 1)[0];
  return rows
  .filter(row => row.filter(cell => cell != '').length)
  .map(row => {
    const obj = {};
    row.map((item, index) => {
      if (Number.isFinite(item)) var value = Number(item);
      else {
        try {
          var value = JSON.parse(item);
        } catch (e) {
            if (e instanceof SyntaxError) { value = item;}
        }
      }
      obj[String(keys[index])] = value;
    });
    return obj;
  });
}

function _getBook(book_id){
  if (book_id){
      return SpreadsheetApp.openById(book_id);
  }
  return SpreadsheetApp.getActive();
}
```
3. Edit key.json to reflect you Google Spreadsheet and Google OAuth data.

## Author

[SHIROKUROHTISUJI](https://shirokurohitsuji.studio/)

## License

**UnityGoogleSpreadsheet** is under [MIT license](https://github.com/shirokurohitsuji/UnityGoogleSpreadsheet/blob/main/LICENSE.md).
