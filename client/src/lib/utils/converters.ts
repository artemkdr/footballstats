/**
 * Converts a given value to an integer.
 *
 * If the value is already a number, it rounds it to the nearest integer.
 * If the value is not a number, it attempts to parse it as an integer with parseInt.
 * If parsing fails, it returns 0.
 *
 * @param value - The value to convert to an integer.
 * @returns The converted integer value.
 */
export const toInt = (value: unknown): number => {
    return typeof(value) == 'number' ? Math.round(value) : (parseInt(String(value)) || 0);
}

/**
 * Converts a given value to a number.
 *
 * If the value is already a number, it returns it as is.
 * If the value is not a number, it attempts to parse it as a number.
 * If parsing fails, it returns 0.
 *
 * @param value - The value to convert to a number.
 * @returns The converted number value.
 */
export const toNumber = (value: unknown): number => {
    return typeof(value) == 'number' ? value : (Number(String(value)) || 0);
}

/**
 * Converts a given value to a string.
 * 
 * @param value - The value to convert to a string.
 * @returns The converted string value.
 */
export const toString = (value: unknown): string => {
    return String(value);
}