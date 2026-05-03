export const useLocale = () => {
  const locale = useState<string>('app-locale', () => 'en-US')

  const setLocale = (l: string) => { locale.value = l }

  const formatNumber = (v: number | string): string =>
    typeof v === 'number' ? v.toLocaleString(locale.value) : v

  return { locale, setLocale, formatNumber }
}