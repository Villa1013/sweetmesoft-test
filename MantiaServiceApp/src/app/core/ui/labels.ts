import { EstadoCotizacion } from '../models/cotizacion.models';
import { EstadoSolicitud, OrigenSolicitud } from '../models/solicitud.models';
import { TipoTarifa } from '../models/tarifa.models';

export type TagSeverity = 'success' | 'info' | 'warn' | 'danger' | 'secondary' | 'contrast';

const ESTADO_SOLICITUD_LABEL: Record<EstadoSolicitud, string> = {
  ListaParaCotizar: 'Lista para cotizar',
  FaltaInformacion: 'Falta información',
  RequiereValoracionTecnica: 'Requiere valoración técnica',
};

const ESTADO_SOLICITUD_SEVERITY: Record<EstadoSolicitud, TagSeverity> = {
  ListaParaCotizar: 'success',
  FaltaInformacion: 'warn',
  RequiereValoracionTecnica: 'danger',
};

const ESTADO_COTIZACION_LABEL: Record<EstadoCotizacion, string> = {
  Borrador: 'Borrador',
  Enviada: 'Enviada',
  Aceptada: 'Aceptada',
  Rechazada: 'Rechazada',
};

const ESTADO_COTIZACION_SEVERITY: Record<EstadoCotizacion, TagSeverity> = {
  Borrador: 'secondary',
  Enviada: 'info',
  Aceptada: 'success',
  Rechazada: 'danger',
};

const ORIGEN_LABEL: Record<OrigenSolicitud, string> = {
  Correo: 'Correo',
  Mensajeria: 'Mensajería',
  PDF: 'PDF',
};

const ORIGEN_ICON: Record<OrigenSolicitud, string> = {
  Correo: 'pi-envelope',
  Mensajeria: 'pi-comments',
  PDF: 'pi-file-pdf',
};

const TIPO_TARIFA_LABEL: Record<TipoTarifa, string> = {
  Servicio: 'Servicio',
  Recargo: 'Recargo',
  Desplazamiento: 'Desplazamiento',
};

export const estadoSolicitudLabel = (e: EstadoSolicitud) => ESTADO_SOLICITUD_LABEL[e] ?? e;
export const estadoSolicitudSeverity = (e: EstadoSolicitud) =>
  ESTADO_SOLICITUD_SEVERITY[e] ?? 'secondary';
export const estadoCotizacionLabel = (e: EstadoCotizacion) => ESTADO_COTIZACION_LABEL[e] ?? e;
export const estadoCotizacionSeverity = (e: EstadoCotizacion) =>
  ESTADO_COTIZACION_SEVERITY[e] ?? 'secondary';
export const origenLabel = (o: OrigenSolicitud) => ORIGEN_LABEL[o] ?? o;
export const origenIcon = (o: OrigenSolicitud) => ORIGEN_ICON[o] ?? 'pi-inbox';
export const tipoTarifaLabel = (t: TipoTarifa) => TIPO_TARIFA_LABEL[t] ?? t;
