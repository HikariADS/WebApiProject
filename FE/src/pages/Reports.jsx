import { useEffect, useState, useMemo } from 'react'
import { productService } from '../api/productService'
import { storageService } from '../api/storageService'
import { productTypeService } from '../api/productTypeService'
import { storageTypeService } from '../api/storageTypeService'
import { useAuth } from '../contexts/AuthContext'
import { handleApiError, logError } from '../utils/errorHandler'
import { normalizeResponse, normalizeArray } from '../utils/dataNormalizer'
import { ERROR_MESSAGES } from '../utils/constants'
import './Reports.css'

const Reports = () => {
  const { user: currentUser } = useAuth()
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')
  const [data, setData] = useState({
    products: [],
    storages: [],
    productTypes: [],
    storageTypes: []
  })

  useEffect(() => {
    fetchAllData()
  }, [])

  const fetchAllData = async () => {
    try {
      setLoading(true)
      const [productsRes, storagesRes, productTypesRes, storageTypesRes] = await Promise.all([
        productService.getAll({ pageSize: 1000 }),
        storageService.getAll(),
        productTypeService.getAll(),
        storageTypeService.getAll()
      ])

      const productsData = normalizeResponse(productsRes)
      setData({
        products: productsData.items || [],
        storages: normalizeArray(storagesRes),
        productTypes: normalizeArray(productTypesRes),
        storageTypes: normalizeArray(storageTypesRes)
      })
      setError('')
    } catch (err) {
      const errorMessage = handleApiError(err, ERROR_MESSAGES.FETCH_FAILED, 'Reports.fetchAllData')
      setError(errorMessage)
      logError(err, 'Reports.fetchAllData')
    } finally {
      setLoading(false)
    }
  }

  // Top sản phẩm có số lượng nhiều nhất trong kho
  const topProductsByQuantity = useMemo(() => {
    const productQuantityMap = new Map()
    
    data.storages.forEach(storage => {
      const productId = storage.productId || storage.ProductId
      const productName = storage.productName || storage.ProductName
      const quantity = storage.quantity || storage.Quantity || 0
      
      if (productId && productName) {
        const current = productQuantityMap.get(productId) || { productId, productName, totalQuantity: 0 }
        current.totalQuantity += quantity
        productQuantityMap.set(productId, current)
      }
    })
    
    return Array.from(productQuantityMap.values())
      .sort((a, b) => b.totalQuantity - a.totalQuantity)
      .slice(0, 10)
  }, [data.storages])

  // Sản phẩm gần hết hàng (số lượng < 50)
  const lowStockProducts = useMemo(() => {
    const productQuantityMap = new Map()
    
    data.storages.forEach(storage => {
      const productId = storage.productId || storage.ProductId
      const productName = storage.productName || storage.ProductName
      const quantity = storage.quantity || storage.Quantity || 0
      
      if (productId && productName) {
        const current = productQuantityMap.get(productId) || { productId, productName, totalQuantity: 0 }
        current.totalQuantity += quantity
        productQuantityMap.set(productId, current)
      }
    })
    
    return Array.from(productQuantityMap.values())
      .filter(p => p.totalQuantity > 0 && p.totalQuantity < 50)
      .sort((a, b) => a.totalQuantity - b.totalQuantity)
      .slice(0, 10)
  }, [data.storages])

  // Kho có số lượng lớn nhất
  const topStorages = useMemo(() => {
    return [...data.storages]
      .sort((a, b) => (b.quantity || b.Quantity || 0) - (a.quantity || a.Quantity || 0))
      .slice(0, 10)
  }, [data.storages])

  // Kho mới nhập gần đây (7 ngày gần nhất)
  const recentImports = useMemo(() => {
    const sevenDaysAgo = new Date()
    sevenDaysAgo.setDate(sevenDaysAgo.getDate() - 7)
    
    return data.storages
      .filter(s => {
        const importDate = s.importDate || s.ImportDate
        if (!importDate) return false
        return new Date(importDate) >= sevenDaysAgo
      })
      .sort((a, b) => {
        const dateA = new Date(a.importDate || a.ImportDate)
        const dateB = new Date(b.importDate || b.ImportDate)
        return dateB - dateA
      })
      .slice(0, 10)
  }, [data.storages])

  // Thống kê theo loại sản phẩm
  const statsByProductType = useMemo(() => {
    const typeMap = new Map()
    
    data.products.forEach(product => {
      const typeId = product.productTypeId || product.ProductTypeId
      const typeName = product.productTypeName || product.ProductTypeName || 'Chưa phân loại'
      
      if (!typeMap.has(typeId)) {
        typeMap.set(typeId, { typeId, typeName, count: 0 })
      }
      typeMap.get(typeId).count++
    })
    
    return Array.from(typeMap.values())
      .sort((a, b) => b.count - a.count)
  }, [data.products])

  // Thống kê theo loại kho
  const statsByStorageType = useMemo(() => {
    const typeMap = new Map()
    
    data.storages.forEach(storage => {
      const typeId = storage.storageTypeId || storage.StorageTypeId
      const typeName = storage.storageTypeName || storage.StorageTypeName || 'Chưa phân loại'
      const quantity = storage.quantity || storage.Quantity || 0
      
      if (!typeMap.has(typeId)) {
        typeMap.set(typeId, { typeId, typeName, count: 0, totalQuantity: 0 })
      }
      const stats = typeMap.get(typeId)
      stats.count++
      stats.totalQuantity += quantity
    })
    
    return Array.from(typeMap.values())
      .sort((a, b) => b.totalQuantity - a.totalQuantity)
  }, [data.storages])

  if (loading) {
    return <div className="loading">Đang tải dữ liệu...</div>
  }

  if (error) {
    return <div className="error">{error}</div>
  }

  return (
    <div className="reports-page">
      <div className="reports-header">
        <h1>Báo cáo & Thống kê</h1>
        <p className="reports-subtitle">Thông tin chi tiết về hệ thống quản lý kho</p>
      </div>

      <div className="reports-grid">
        {/* Top sản phẩm theo số lượng */}
        <div className="report-card">
          <div className="report-card-header">
            <h2>📈 Top 10 sản phẩm có số lượng nhiều nhất</h2>
          </div>
          <div className="report-card-body">
            {topProductsByQuantity.length === 0 ? (
              <p className="no-data">Chưa có dữ liệu</p>
            ) : (
              <div className="report-list">
                {topProductsByQuantity.map((item, index) => (
                  <div key={item.productId} className="report-item">
                    <div className="report-rank">#{index + 1}</div>
                    <div className="report-info">
                      <div className="report-name">{item.productName}</div>
                      <div className="report-value">
                        {item.totalQuantity.toLocaleString('vi-VN')} sản phẩm
                      </div>
                    </div>
                  </div>
                ))}
              </div>
            )}
          </div>
        </div>

        {/* Sản phẩm gần hết hàng */}
        <div className="report-card warning">
          <div className="report-card-header">
            <h2>⚠️ Sản phẩm gần hết hàng</h2>
          </div>
          <div className="report-card-body">
            {lowStockProducts.length === 0 ? (
              <p className="no-data">Không có sản phẩm nào gần hết hàng</p>
            ) : (
              <div className="report-list">
                {lowStockProducts.map((item, index) => (
                  <div key={item.productId} className="report-item">
                    <div className="report-rank warning">#{index + 1}</div>
                    <div className="report-info">
                      <div className="report-name">{item.productName}</div>
                      <div className="report-value warning">
                        {item.totalQuantity.toLocaleString('vi-VN')} sản phẩm
                      </div>
                    </div>
                  </div>
                ))}
              </div>
            )}
          </div>
        </div>

        {/* Kho có số lượng lớn nhất */}
        <div className="report-card">
          <div className="report-card-header">
            <h2>🏪 Top 10 kho có số lượng lớn nhất</h2>
          </div>
          <div className="report-card-body">
            {topStorages.length === 0 ? (
              <p className="no-data">Chưa có dữ liệu</p>
            ) : (
              <div className="report-list">
                {topStorages.map((item, index) => (
                  <div key={item.id || item.Id} className="report-item">
                    <div className="report-rank">#{index + 1}</div>
                    <div className="report-info">
                      <div className="report-name">
                        {item.productName || item.ProductName || 'N/A'} - {item.storageTypeName || item.StorageTypeName || 'N/A'}
                      </div>
                      <div className="report-value">
                        {(item.quantity || item.Quantity || 0).toLocaleString('vi-VN')} sản phẩm
                      </div>
                      <div className="report-location">
                        📍 {item.storageLocation || item.StorageLocation || 'Chưa có vị trí'}
                      </div>
                    </div>
                  </div>
                ))}
              </div>
            )}
          </div>
        </div>

        {/* Kho mới nhập gần đây */}
        <div className="report-card">
          <div className="report-card-header">
            <h2>🆕 Kho mới nhập (7 ngày gần nhất)</h2>
          </div>
          <div className="report-card-body">
            {recentImports.length === 0 ? (
              <p className="no-data">Không có kho nào được nhập trong 7 ngày qua</p>
            ) : (
              <div className="report-list">
                {recentImports.map((item, index) => (
                  <div key={item.id || item.Id} className="report-item">
                    <div className="report-rank">#{index + 1}</div>
                    <div className="report-info">
                      <div className="report-name">
                        {item.productName || item.ProductName || 'N/A'}
                      </div>
                      <div className="report-value">
                        {(item.quantity || item.Quantity || 0).toLocaleString('vi-VN')} sản phẩm
                      </div>
                      <div className="report-date">
                        📅 {item.importDate ? new Date(item.importDate).toLocaleDateString('vi-VN') : 'N/A'}
                      </div>
                    </div>
                  </div>
                ))}
              </div>
            )}
          </div>
        </div>

        {/* Thống kê theo loại sản phẩm */}
        <div className="report-card">
          <div className="report-card-header">
            <h2>🏷️ Thống kê theo loại sản phẩm</h2>
          </div>
          <div className="report-card-body">
            {statsByProductType.length === 0 ? (
              <p className="no-data">Chưa có dữ liệu</p>
            ) : (
              <div className="report-list">
                {statsByProductType.map((item) => (
                  <div key={item.typeId} className="report-item">
                    <div className="report-info">
                      <div className="report-name">{item.typeName}</div>
                      <div className="report-value">
                        {item.count} sản phẩm
                      </div>
                    </div>
                  </div>
                ))}
              </div>
            )}
          </div>
        </div>

        {/* Thống kê theo loại kho */}
        <div className="report-card">
          <div className="report-card-header">
            <h2>📋 Thống kê theo loại kho</h2>
          </div>
          <div className="report-card-body">
            {statsByStorageType.length === 0 ? (
              <p className="no-data">Chưa có dữ liệu</p>
            ) : (
              <div className="report-list">
                {statsByStorageType.map((item) => (
                  <div key={item.typeId} className="report-item">
                    <div className="report-info">
                      <div className="report-name">{item.typeName}</div>
                      <div className="report-value">
                        {item.count} kho - {item.totalQuantity.toLocaleString('vi-VN')} sản phẩm
                      </div>
                    </div>
                  </div>
                ))}
              </div>
            )}
          </div>
        </div>
      </div>
    </div>
  )
}

export default Reports

