export type EstadoCotizacion = 'Borrador' | 'Enviada' | 'Aceptada' | 'Rechazada';

export interface LineaCotizacion {
  codigo: string;
  concepto: string;
  cantidad: number;
  precioUnitarioCOP: number;
  valorLineaCOP: number;
}

export interface CotizacionListado {
  id: number;
  numero: string;
  fechaEmision: string;
  cliente: string;
  municipio: string;
  estado: EstadoCotizacion;
  subtotal: number;
  iva: number;
  total: number;
  versionTarifa: string;
  esHistorica: boolean;
  solicitudId: number | null;
}

export interface CotizacionDetalle {
  id: number;
  numero: string;
  fechaEmision: string;
  validaHasta: string;
  solicitudId: number | null;
  cliente: string;
  municipio: string;
  direccion: string;
  versionTarifa: string;
  estado: EstadoCotizacion;
  subtotal: number;
  iva: number;
  total: number;
  porcentajeIVA: number;
  observaciones: string;
  esHistorica: boolean;
  lineas: LineaCotizacion[];
}

export interface ErrorCalculo {
  errores: string[];
}

export const ESTADOS_COTIZACION: EstadoCotizacion[] = [
  'Borrador',
  'Enviada',
  'Aceptada',
  'Rechazada',
];
