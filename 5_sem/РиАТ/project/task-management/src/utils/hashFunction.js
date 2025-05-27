import CryptoJS from 'crypto-js';

export function hashFunction(str) {
    return CryptoJS.SHA256(str).toString(CryptoJS.enc.Hex);
}