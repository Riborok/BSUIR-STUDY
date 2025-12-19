export function formatDateISO(date: Date | number | string): string {
    const d = date instanceof Date ? date : new Date(date)
    return d.toISOString()
}

export function formatDateHuman(date: Date | number | string): string {
    const d = date instanceof Date ? date : new Date(date)
    return d.toLocaleString()
}
