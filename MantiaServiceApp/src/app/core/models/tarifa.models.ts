export type TipoTarifa = 'Servicio' | 'Recargo' | 'Desplazamiento';

export interface Tarifa {
  version: string;
  codigo: string;
  concepto: string;
  unidad: string;
  precioUnitarioCOP: number;
  tipo: TipoTarifa;
}
