
const styleSheets = {};

export function createStyleSheet(id) {
    const styleSheetsElement = document.createElement('style');
    document.getElementsByTagName('head')[0].appendChild(styleSheetsElement);
    const styleSheet = styleSheetsElement.sheet;
    styleSheets[id] = {
        element: styleSheetsElement,
        sheet: styleSheet
    };
    return true;
}

export function addClass(id, selector, rules) {
    const styleSheet = styleSheets[id];
    if (!styleSheet) {
        return false;
    }
    styleSheet.sheet.insertRule(selector + rules, styleSheet.sheet.cssRules.length);
    return true;
}