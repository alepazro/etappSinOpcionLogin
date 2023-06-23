var cards = new Array();
cards[0] = { name: "MC", clength: [16], exp: /^5[1-5]/ };
cards[1] = { name: "VISA", clength: [13, 16], exp: /^4/ };
cards[2] = { name: "AMEX", clength: [15], exp: /^3(4|7)/ };
cards[3] = { name: "dnr", clength: [14], exp: /^3[0,6,8]\d{12}/ };
cards[4] = { name: "DIS", clength: [16], exp: /^6011\d{4}\d{4}\d{4}/ };

function ValidateCreditCard() {
    var cardType = $('.cc-ddl-type').val();
    var cardNumber = $('.cc-card-number').val();

    var isValid = false;
    var ccCheckRegExp = /[^\d ]/;
    isValid = !ccCheckRegExp.test(cardNumber);
    if (isValid) {
        var cardNumbersOnly = cardNumber.replace(/ /g, ""); //trim spaces
        var cardNumberLength = cardNumbersOnly.length;
        var lengthIsValid = false;
        var prefixIsValid = false;
        var prefixRegExp;
        var card = getCardType(cardType);
        if (card != null) {
            for (i = 0; i < card.clength.length; i++) {
                if (!lengthIsValid) {
                    if (cardNumberLength == card.clength[i]) lengthIsValid = true;
                }
            }
            prefixIsValid = card.exp.test(cardNumbersOnly);
        }
        isValid = prefixIsValid && lengthIsValid;
    }
    if (isValid) {
        var checkSumTotal = 0;
        checkSumTotal = computeChecksum(cardNumbersOnly);
        isValid = (checkSumTotal % 10 == 0);
    }
    return isValid;
}

function getCardType(type) {
    var card = null; ;
    for (i = 0; i < cards.length; i++) {
        if (cards[i].name.toLowerCase() == type.toLowerCase()) {
            card = cards[i];
            break;
        }
    }
    return card;
}

function computeChecksum(cardNo) {
    var checksum = 0;
    var factor = 1;
    var temp;
    for (i = cardNo.length - 1; i >= 0; i--) {
        temp = Number(cardNo.charAt(i)) * factor;
        if (temp > 9) {
            checksum += 1;
            temp -= 10;
        }
        checksum += temp;
        factor = (factor == 1 ? 2 : 1);
    }
    return checksum;
}
