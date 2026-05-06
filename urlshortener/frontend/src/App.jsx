import { useState, useEffect, useCallback } from 'react'
import { Toaster, toast } from 'react-hot-toast'
import {
  Link2, Scissors, BarChart2, Copy, Trash2, ExternalLink,
  RefreshCw, ChevronLeft, ChevronRight, Power, Zap, Clock, Search
} from 'lucide-react'
import { urlApi } from './api'
import styles from './App.module.css'

/* ─────────────── helpers ─────────────── */
const fmt = (iso) => new Date(iso).toLocaleDateString('en-US', {
  month: 'short', day: 'numeric', year: 'numeric'
})
const fmtNum = (n) => n >= 1000 ? `${(n / 1000).toFixed(1)}k` : String(n)

/* ─────────────── CreateForm ─────────────── */
function CreateForm({ onCreated }) {
  const [url, setUrl] = useState('')
  const [title, setTitle] = useState('')
  const [custom, setCustom] = useState('')
  const [loading, setLoading] = useState(false)
  const [result, setResult] = useState(null)

  const handleSubmit = async (e) => {
    e.preventDefault()
    if (!url.trim()) return
    setLoading(true)
    setResult(null)
    try {
      const data = await urlApi.create({
        originalUrl: url.trim(),
        title: title.trim() || undefined,
        customCode: custom.trim() || undefined,
      })
      setResult(data)
      onCreated(data)
      toast.success('URL shortened!', { icon: '✂️' })
      setUrl(''); setTitle(''); setCustom('')
    } catch (err) {
      const msg = err.response?.data?.message || 'Failed to shorten URL.'
      toast.error(msg)
    } finally {
      setLoading(false)
    }
  }

  const copy = (text) => {
    navigator.clipboard.writeText(text)
    toast.success('Copied to clipboard!')
  }

  return (
    <div className={styles.card}>
      <div className={styles.cardHeader}>
        <Scissors size={20} className={styles.cardIcon} />
        <h2>Shorten a URL</h2>
      </div>

      <form onSubmit={handleSubmit} className={styles.form}>
        <div className={styles.inputGroup}>
          <label>URL *</label>
          <div className={styles.inputWrap}>
            <Link2 size={16} className={styles.inputIcon} />
            <input
              type="url"
              placeholder="https://example.com/very/long/url"
              value={url}
              onChange={e => setUrl(e.target.value)}
              required
              className={styles.input}
            />
          </div>
        </div>

        <div className={styles.row}>
          <div className={styles.inputGroup}>
            <label>Title (optional)</label>
            <input
              type="text"
              placeholder="My awesome link"
              value={title}
              onChange={e => setTitle(e.target.value)}
              className={styles.input}
            />
          </div>
          <div className={styles.inputGroup}>
            <label>Custom code (optional)</label>
            <div className={styles.inputWrap}>
              <span className={styles.inputPrefix}>snip/</span>
              <input
                type="text"
                placeholder="my-brand"
                value={custom}
                onChange={e => setCustom(e.target.value.replace(/[^a-zA-Z0-9\-_]/g, ''))}
                className={`${styles.input} ${styles.inputPrefixed}`}
                maxLength={20}
              />
            </div>
          </div>
        </div>

        <button type="submit" className={styles.btnPrimary} disabled={loading}>
          {loading ? <RefreshCw size={16} className={styles.spin} /> : <Zap size={16} />}
          {loading ? 'Shortening…' : 'Shorten URL'}
        </button>
      </form>

      {result && (
        <div className={styles.resultBox}>
          <div className={styles.resultLabel}>Your short URL is ready</div>
          <div className={styles.resultUrl}>
            <span className="mono">{result.shortUrl}</span>
            <div className={styles.resultActions}>
              <button onClick={() => copy(result.shortUrl)} className={styles.iconBtn} title="Copy">
                <Copy size={15} />
              </button>
              <a href={result.shortUrl} target="_blank" rel="noreferrer" className={styles.iconBtn} title="Open">
                <ExternalLink size={15} />
              </a>
            </div>
          </div>
        </div>
      )}
    </div>
  )
}

/* ─────────────── URLTable ─────────────── */
function URLTable({ urls, onDelete, onToggle, loading }) {
  const copy = (text) => {
    navigator.clipboard.writeText(text)
    toast.success('Copied!')
  }

  if (loading) return (
    <div className={styles.emptyState}>
      <RefreshCw size={32} className={styles.spin} style={{ opacity: 0.4 }} />
    </div>
  )

  if (!urls.length) return (
    <div className={styles.emptyState}>
      <Link2 size={40} style={{ opacity: 0.2 }} />
      <p>No URLs yet. Shorten one above!</p>
    </div>
  )

  return (
    <div className={styles.table}>
      <div className={styles.tableHead}>
        <span>URL / TITLE</span>
        <span>SHORT CODE</span>
        <span>CLICKS</span>
        <span>CREATED</span>
        <span>STATUS</span>
        <span>ACTIONS</span>
      </div>
      {urls.map(row => (
        <div key={row.id} className={`${styles.tableRow} ${!row.isActive ? styles.inactive : ''}`}>
          <div className={styles.urlCell}>
            <span className={styles.urlTitle}>{row.title || 'Untitled'}</span>
            <span className={styles.urlOrig}>{row.originalUrl}</span>
          </div>
          <div className="mono" style={{ fontSize: 13 }}>
            <span className={styles.code}>{row.code}</span>
          </div>
          <div className={styles.clicks}>
            <BarChart2 size={13} />
            {fmtNum(row.clickCount)}
          </div>
          <div style={{ fontSize: 12, color: 'var(--text3)' }}>
            <Clock size={12} style={{ marginRight: 4 }} />
            {fmt(row.createdAt)}
          </div>
          <div>
            <span className={`${styles.badge} ${row.isActive ? styles.badgeActive : styles.badgeOff}`}>
              {row.isActive ? 'Active' : 'Off'}
            </span>
          </div>
          <div className={styles.actions}>
            <button onClick={() => copy(row.shortUrl)} className={styles.iconBtn} title="Copy short URL">
              <Copy size={14} />
            </button>
            <a href={row.originalUrl} target="_blank" rel="noreferrer" className={styles.iconBtn} title="Open original">
              <ExternalLink size={14} />
            </a>
            <button onClick={() => onToggle(row)} className={styles.iconBtn} title={row.isActive ? 'Disable' : 'Enable'}>
              <Power size={14} />
            </button>
            <button onClick={() => onDelete(row.id)} className={`${styles.iconBtn} ${styles.danger}`} title="Delete">
              <Trash2 size={14} />
            </button>
          </div>
        </div>
      ))}
    </div>
  )
}

/* ─────────────── App ─────────────── */
export default function App() {
  const [urls, setUrls] = useState([])
  const [totalCount, setTotalCount] = useState(0)
  const [page, setPage] = useState(1)
  const [loading, setLoading] = useState(false)
  const [search, setSearch] = useState('')
  const PAGE_SIZE = 10

  const load = useCallback(async (p = 1) => {
    setLoading(true)
    try {
      const data = await urlApi.getAll(p, PAGE_SIZE)
      setUrls(data.items)
      setTotalCount(data.totalCount)
    } catch {
      toast.error('Failed to load URLs')
    } finally {
      setLoading(false)
    }
  }, [])

  useEffect(() => { load(page) }, [page, load])

  const handleCreated = () => { setPage(1); load(1) }

  const handleDelete = async (id) => {
    if (!confirm('Delete this URL?')) return
    try {
      await urlApi.delete(id)
      toast.success('Deleted')
      load(page)
    } catch { toast.error('Failed to delete') }
  }

  const handleToggle = async (row) => {
    try {
      await urlApi.update(row.id, { isActive: !row.isActive })
      toast.success(row.isActive ? 'Disabled' : 'Enabled')
      load(page)
    } catch { toast.error('Failed to update') }
  }

  const filtered = search
    ? urls.filter(u =>
        u.code.includes(search) ||
        u.originalUrl.toLowerCase().includes(search.toLowerCase()) ||
        (u.title || '').toLowerCase().includes(search.toLowerCase())
      )
    : urls

  const totalPages = Math.ceil(totalCount / PAGE_SIZE)

  return (
    <div className={styles.app}>
      <Toaster position="top-right" toastOptions={{
        style: { background: 'var(--bg3)', color: 'var(--text)', border: '1px solid var(--border)' }
      }} />

      {/* Header */}
      <header className={styles.header}>
        <div className={styles.logo}>
          <Scissors size={22} className={styles.logoIcon} />
          <span>snip<em>.io</em></span>
        </div>
        <div className={styles.headerStats}>
          <span>{totalCount} links shortened</span>
        </div>
      </header>

      {/* Hero */}
      <div className={styles.hero}>
        <div className={styles.heroBadge}>⚡ Fast · Reliable · Open Source</div>
        <h1 className={styles.heroTitle}>
          Shorten.<br />
          <span className={styles.gradient}>Share.</span> Track.
        </h1>
        <p className={styles.heroSub}>
          Create short, memorable links in seconds. Track clicks and manage your links — all in one place.
        </p>
      </div>

      <main className={styles.main}>
        {/* Create form */}
        <CreateForm onCreated={handleCreated} />

        {/* Manage section */}
        <div className={styles.card} style={{ marginTop: 0 }}>
          <div className={styles.cardHeader}>
            <BarChart2 size={20} className={styles.cardIcon} />
            <h2>Manage URLs</h2>
            <div className={styles.searchWrap}>
              <Search size={14} />
              <input
                type="text"
                placeholder="Filter..."
                value={search}
                onChange={e => setSearch(e.target.value)}
                className={styles.searchInput}
              />
            </div>
            <button onClick={() => load(page)} className={styles.iconBtn} title="Refresh">
              <RefreshCw size={15} className={loading ? styles.spin : ''} />
            </button>
          </div>

          <URLTable urls={filtered} onDelete={handleDelete} onToggle={handleToggle} loading={loading} />

          {totalPages > 1 && (
            <div className={styles.pagination}>
              <button
                className={styles.iconBtn}
                onClick={() => setPage(p => Math.max(1, p - 1))}
                disabled={page === 1}
              >
                <ChevronLeft size={16} />
              </button>
              <span className={styles.pageInfo}>Page {page} of {totalPages}</span>
              <button
                className={styles.iconBtn}
                onClick={() => setPage(p => Math.min(totalPages, p + 1))}
                disabled={page === totalPages}
              >
                <ChevronRight size={16} />
              </button>
            </div>
          )}
        </div>
      </main>

      <footer className={styles.footer}>
        Built with .NET 9 + React · MIT License
      </footer>
    </div>
  )
}
